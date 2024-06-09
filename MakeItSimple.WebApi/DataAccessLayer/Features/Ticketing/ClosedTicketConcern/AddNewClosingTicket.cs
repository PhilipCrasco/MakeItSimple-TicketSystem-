using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;
using System.Security.Policy;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class AddNewClosingTicket
    {
        public class AddNewClosingTicketCommand : IRequest<Result>
        {
            public string Closed_Remarks { get; set; }
            public  Guid ? Modified_By { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }
            public int ? TicketConcernId { get; set; }
            public int? ClosingTicketId { get; set; }
            public string Resolution { get; set; } 
            public List<AddClosingAttachment> AddClosingAttachments { get; set; }

            public class AddClosingAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }

            }
             
            public class Handler : IRequestHandler<AddNewClosingTicketCommand, Result>
            {
                private readonly MisDbContext _context;
                private readonly Cloudinary _cloudinary;
                private readonly TransformUrl _url;

                public Handler(MisDbContext context, IOptions<CloudinaryOption> options, TransformUrl url)
                {
                    _context = context;
                    var account = new Account(
                        options.Value.Cloudname,
                        options.Value.ApiKey,
                        options.Value.ApiSecret
                        );
                    _cloudinary = new Cloudinary(account);
                    _url = url;
                }

                public async Task<Result> Handle(AddNewClosingTicketCommand command, CancellationToken cancellationToken)
                {

                    var userDetails = await _context.Users
                       .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                    var ticketConcernExist = await _context.TicketConcerns
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId, cancellationToken);

                    if (ticketConcernExist == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());
                    }

                    var closingTicketExist = await _context.ClosingTickets
                        .FirstOrDefaultAsync(x => x.Id == command.ClosingTicketId);

                    if (closingTicketExist is not null)
                    {

                        bool IsChanged = false;

                        if(closingTicketExist.Resolution != command.Resolution)
                        {
                            closingTicketExist.Resolution = command.Resolution;
                            IsChanged = true;
                        }

                        if (IsChanged)
                        {
                            closingTicketExist.ModifiedBy = command.Modified_By;
                            closingTicketExist.UpdatedAt = DateTime.Now;
                        }

                    }
                    else
                    {

                        var approverByUser = await _context.Users
                                .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.UserId, cancellationToken);

                        var approverList = await _context.Approvers
                            .Where(x => x.SubUnitId == approverByUser.SubUnitId)
                            .ToListAsync();

                        if (approverList.Count() < 0)
                        {
                            return Result.Failure(ClosingTicketError.NoApproverHasSetup());
                        }

                        var approverUser = approverList.First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                        var addNewClosingConcern = new ClosingTicket
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            Resolution = command.Resolution,
                            IsClosing = false,
                            TicketApprover = approverUser.UserId,
                            AddedBy = command.Added_By
                        };

                        await _context.ClosingTickets.AddAsync(addNewClosingConcern);
                        await _context.SaveChangesAsync(cancellationToken);

                        ticketConcernExist.IsClosedApprove = false;
                        closingTicketExist = addNewClosingConcern;

                        var getApprover = await _context.Approvers
                        .Where(x => x.SubUnitId == ticketConcernExist.User.SubUnitId)
                        .ToListAsync();

                        if (getApprover == null)
                        {
                            return Result.Failure(TransferTicketError.NoApproverExist());
                        }

                        foreach (var approver in getApprover)
                        {
                            var addNewApprover = new ApproverTicketing
                            {
                                TicketConcernId = ticketConcernExist.Id,
                                ClosingTicketId = addNewClosingConcern.Id,
                                UserId = approver.UserId,
                                ApproverLevel = approver.ApproverLevel,
                                AddedBy = command.Added_By,
                                CreatedAt = DateTime.Now,
                                Status = TicketingConString.CloseTicket,

                            };

                            await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                        }

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            RequestorBy = command.Requestor_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.CloseTicket,
                            Status = TicketingConString.RequestCreated
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    }


                    var uploadTasks = new List<Task>();

                    if (command.AddClosingAttachments.Count(x => x.Attachment != null) > 0)
                    {

                        foreach (var attachments in command.AddClosingAttachments.Where(attachments => attachments.Attachment.Length > 0))
                        {

                            var ticketAttachment = await _context.TicketAttachments
                             .FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

                            if (attachments.Attachment == null || attachments.Attachment.Length == 0)
                            {
                                return Result.Failure(TicketRequestError.AttachmentNotNull());
                            }

                            if (attachments.Attachment.Length > 10 * 1024 * 1024)
                            {
                                return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                            }

                            var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                            var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                            if (extension == null || !allowedFileTypes.Contains(extension))
                            {
                                return Result.Failure(TicketRequestError.InvalidAttachmentType());
                            }

                            uploadTasks.Add(Task.Run(async () =>
                            {
                                await using var stream = attachments.Attachment.OpenReadStream();

                                var attachmentsParams = new RawUploadParams
                                {
                                    File = new FileDescription(attachments.Attachment.FileName, stream),
                                    PublicId = $"MakeITSimple/Ticketing/Closing/{userDetails.Fullname}/{attachments.Attachment.FileName}"
                                };

                                var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);
                                string attachmentUrl = attachmentResult.SecureUrl.ToString();
                                string transformedUrl = _url.TransformUrlForViewOnly(attachmentUrl, attachments.Attachment.FileName);

                                if (ticketAttachment != null)
                                {
                                    var hasChanged = false;

                                    if (ticketAttachment.Attachment != attachmentResult.SecureUrl.ToString())
                                    {
                                        ticketAttachment.Attachment = attachmentResult.SecureUrl.ToString();
                                        hasChanged = true;
                                    }

                                    if (hasChanged)
                                    {
                                        ticketAttachment.ModifiedBy = command.Modified_By;
                                        ticketAttachment.FileName = attachments.Attachment.FileName;
                                        ticketAttachment.FileSize = attachments.Attachment.Length;
                                        ticketAttachment.UpdatedAt = DateTime.Now;

                                    }

                                }
                                else
                                {
                                    var addAttachment = new TicketAttachment
                                    {
                                        ClosingTicketId = closingTicketExist.Id,
                                        Attachment = attachmentResult.SecureUrl.ToString(),
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        AddedBy = command.Added_By,

                                    };

                                    await _context.TicketAttachments.AddAsync(addAttachment);

                                }


                            }, cancellationToken));

                        }
                    }
 

                    await Task.WhenAll(uploadTasks);

                    await _context.SaveChangesAsync(cancellationToken);

                    return Result.Success();


                }
            }

        }
    }
}
