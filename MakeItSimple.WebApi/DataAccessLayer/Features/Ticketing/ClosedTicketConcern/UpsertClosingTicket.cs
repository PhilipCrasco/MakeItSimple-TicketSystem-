using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MakeItSimple.WebApi.Common.Cloudinary;
using Microsoft.Extensions.Options;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class UpsertClosingTicket
    {
        public class UpsertClosingTicketCommand : IRequest<Result>
        {
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int? TicketTransactionId { get; set; }
            public Guid? Requestor_By { get; set; }
            public string Role { get; set; }
            public string Remarks { get; set; }

            public List<UpsertClosingTicketConcern> UpsertClosingTicketConcerns {  get; set; }
            public List<UpsertClosingAttachment> UpsertClosingAttachments { get; set; }
            public class UpsertClosingTicketConcern
            {
                public int TicketConcernId { get; set; }
                public int? ClosingTicketId { get; set; }
            }

            public class UpsertClosingAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }

            }

            public class Handler : IRequestHandler<UpsertClosingTicketCommand, Result>
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

                public async Task<Result> Handle(UpsertClosingTicketCommand command, CancellationToken cancellationToken)
                {

                    //var closeHistoryList = new List<ClosingTicket>();
                    var updateClosingList = new List<ClosingTicket>();
                    var updateAttachmentList = new List<TicketAttachment>();
                    var removeTicketConcern = new List<UpsertClosingTicketConcern>();

                    var userDetails = await _context.Users
                       .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                    var allUserList = await _context.UserRoles.ToListAsync();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                    var ticketTransactionExist = await _context.ClosingTickets
                        .FirstOrDefaultAsync(x => x.TicketTransactionId == command.TicketTransactionId, cancellationToken);

                    if (ticketTransactionExist == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    var requestClosingList = await _context.ClosingTickets
                        .Where(x => x.TicketTransactionId == command.TicketTransactionId)
                        .ToListAsync();

                    var validateApprover = await _context.ApproverTicketings
                        .FirstOrDefaultAsync(x => x.TicketTransactionId == ticketTransactionExist.Id
                    && x.IsApprove != null, cancellationToken);

                    if (validateApprover is not null && !receiverPermissionList.Any(x => x.Contains(command.Role)))
                    {
                        return Result.Failure(ClosingTicketError.ClosingTicketConcernUnable());
                    }

                    foreach( var close in command.UpsertClosingTicketConcerns )
                    {
                        var ticketConcernExist = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.Id == close.TicketConcernId, cancellationToken);

                        if (ticketConcernExist == null)
                        {
                            return Result.Failure(TransferTicketError.TicketConcernIdNotExist());
                        }

                        if (command.UpsertClosingTicketConcerns.Count(x => x.TicketConcernId == close.TicketConcernId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateConcernTicket());
                        }

                        if (command.UpsertClosingTicketConcerns.Count(x => x.ClosingTicketId == close.ClosingTicketId) > 1)
                        {
                            return Result.Failure(TransferTicketError.DuplicateTransferTicket());
                        }

                        var closingTicketConcern = await _context.ClosingTickets
                            .FirstOrDefaultAsync(x => x.Id == close.ClosingTicketId, cancellationToken);

                        if(closingTicketConcern is null)
                        {

                            var addClosingTicket = new ClosingTicket
                            {
                                TicketTransactionId = ticketTransactionExist.Id,
                                TicketConcernId = ticketConcernExist.Id,
                                IsClosing = false,
                                TicketApprover = ticketTransactionExist.TicketApprover
                            };

                            await _context.ClosingTickets.AddAsync(addClosingTicket, cancellationToken);

                            if (requestClosingList.Any(x => x.IsRejectClosed is true))
                            {
                                updateClosingList.Add(addClosingTicket);
                            }
                        }

                        removeTicketConcern.Add(close);
                    }


                    var uploadTasks = new List<Task>();

                    if (command.UpsertClosingAttachments.Count(x => x.Attachment != null) > 0)
                    {
                        foreach (var attachments in command.UpsertClosingAttachments.Where(attachments => attachments.Attachment.Length > 0))
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
                                    PublicId = $"MakeITSimple/Ticketing/Closing/{userDetails.Fullname}/{attachments.Attachment.FileName}",
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

                                        if (requestClosingList.Any(x => x.IsRejectClosed is true))
                                        {
                                            updateAttachmentList.Add(ticketAttachment);
                                        }

                                    }

                                }
                                else
                                {
                                    var addAttachment = new TicketAttachment
                                    {
                                        RequestTransactionId = ticketTransactionExist.TicketTransactionId,
                                        Attachment = attachmentResult.SecureUrl.ToString(),
                                        FileName = attachments.Attachment.FileName,
                                        FileSize = attachments.Attachment.Length,
                                        AddedBy = command.Added_By,

                                    };

                                    await _context.TicketAttachments.AddAsync(addAttachment);

                                    if (requestClosingList.Any(x => x.IsRejectClosed is true))
                                    {
                                        updateAttachmentList.Add(ticketAttachment);
                                    }

                                }

                            }, cancellationToken));

                        }
                    }


                    var selectRemoveConcern = removeTicketConcern.Select(x => x.ClosingTicketId);

                    var removeConcernList = await _context.ClosingTickets
                        .Where(x => !selectRemoveConcern.Contains(x.Id)
                        && x.TicketTransactionId == ticketTransactionExist.TicketTransactionId)
                        .ToListAsync();

                    if(removeConcernList.Any())
                    {
                        foreach (var removeConcern in removeConcernList)
                        {
                            removeConcern.IsActive = false;

                            var addTicketHistory = new TicketHistory
                            {
                                TicketConcernId = removeConcern.Id,
                                RequestorBy = command.Requestor_By,
                                TransactionDate = DateTime.Now,
                                Request = TicketingConString.CloseTicket,
                                Status = TicketingConString.Cancel
                            };

                            await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                            if(requestClosingList.Any(x => x.IsRejectClosed is true))
                            {
                                updateClosingList.Add(removeConcern);
                            }

                            var ticketConcernExist = await _context.TicketConcerns
                               .FirstOrDefaultAsync(x => x.Id == removeConcern.TicketConcernId, cancellationToken);

                            ticketTransactionExist.IsClosing = null;
                        }
                    }

                    if (updateAttachmentList.Any() || updateClosingList.Any())
                    { 

                        foreach (var closeTicket in requestClosingList)
                        {
                            closeTicket.Remarks = command.Remarks;
                            closeTicket.RejectRemarks = null;
                            closeTicket.RejectClosedBy = null;
                            closeTicket.IsRejectClosed = false;
                            closeTicket.RejectClosedAt = null;

                            var addTicketHistory = new TicketHistory
                            {
                                TicketConcernId = closeTicket.TicketConcernId,
                                RequestorBy = command.Requestor_By,
                                TransactionDate = DateTime.Now,
                                Request = TicketingConString.CloseTicket,
                                Status = TicketingConString.RequestUpdate
                            };

                            await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
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
