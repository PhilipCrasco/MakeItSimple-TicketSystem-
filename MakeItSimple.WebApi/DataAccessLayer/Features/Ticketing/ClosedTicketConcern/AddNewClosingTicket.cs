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
            public Guid ? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }

            public List<AddClosingTicketConcern> AddClosingTicketConcerns { get; set; }
            public List<AddClosingAttachment> AddClosingAttachments { get; set; }

            public class AddClosingTicketConcern
            {
                public int TicketConcernId { get; set; }
            }

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

                    var ticketConcernList = new List<TicketConcern>();
                    var ticketTransactionExist = new TicketTransaction { IsActive = true };

                    await _context.TicketTransactions.AddAsync(ticketTransactionExist, cancellationToken);

                     var userDetails = await _context.Users
                        .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                    foreach (var close in command.AddClosingTicketConcerns)
                    {
                        var ticketConcernExist = await _context.TicketConcerns
                            .Include(x => x.User)
                            .FirstOrDefaultAsync(x => x.Id == close.TicketConcernId, cancellationToken);

                        if (ticketConcernExist == null)
                        {
                            return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());
                        }

                        var approverByUser = await _context.Users
                            .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.UserId, cancellationToken);

                        var approverList = await _context.Approvers
                            .Where(x => x.SubUnitId == approverByUser.SubUnitId)
                            .ToListAsync();
                        
                        if(approverList.Count() < 0)
                        {
                            return Result.Failure(ClosingTicketError.NoApproverHasSetup());
                        }

                        ticketConcernList.Add(ticketConcernExist);
                        await _context.SaveChangesAsync(cancellationToken);

                        var approverUser = approverList.First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                        var addNewClosingConcern = new ClosingTicket
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TicketTransactionId = ticketTransactionExist.Id,
                            IsClosing = false,
                            TicketApprover = approverUser.UserId,
                            AddedBy = command.Added_By
                        };

                        await _context.ClosingTickets.AddAsync(addNewClosingConcern);

                        ticketConcernExist.IsClosedApprove = false;

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

                                var addAttachment = new TicketAttachment
                                {
                                    TicketTransactionId = ticketTransactionExist.Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.AddAsync(addAttachment, cancellationToken);


                            }, cancellationToken));

                        }
                    }

                    var getApprover = await _context.Approvers
                    .Where(x => x.SubUnitId == ticketConcernList.First().User.SubUnitId)
                    .ToListAsync();

                    if (getApprover == null)
                    {
                        return Result.Failure(TransferTicketError.NoApproverExist());
                    }

                    foreach (var approver in getApprover)
                    {
                        var addNewApprover = new ApproverTicketing
                        {
                            TicketTransactionId = ticketTransactionExist.Id,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            Status = TicketingConString.CloseTicket,

                        };

                        await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                    }


                    await Task.WhenAll(uploadTasks);

                    await _context.SaveChangesAsync(cancellationToken);

                    return Result.Success();


                }
            }

        }
    }
}
