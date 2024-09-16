using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddRequestConcern
    {

        public class AddRequestConcernCommand : IRequest<Result>
        {
           
             
            public Guid ? Added_By { get; set; } 
            public Guid ? Modified_By { get; set; }
            public Guid ? UserId { get; set; }
            public int? RequestConcernId { get; set; }
            public string Concern { get; set; }
            public string Remarks { get; set; }

            public List<RequestAttachmentsFile> RequestAttachmentsFiles {  get; set; }

            public class RequestAttachmentsFile
            {
                public int ? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }


            }

        }

        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;
            private readonly TransformUrl _url;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options , TransformUrl url)
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

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var updateRequestList = new List<RequestConcern>();
                var updateRequestAttachmentList = new List<TicketAttachment>();
                var ticketConcernList = new List<TicketConcern>();

                //var yearSuffix = DateTime.Now.Year % 100;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var userId = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId);
                if (userId == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }


                var requestConcernIdExist = await _context.RequestConcerns
                    .Include(x => x.User)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);

                if (requestConcernIdExist != null)
                {

                    var ticketConcernExist = await _context.TicketConcerns
                        .FirstOrDefaultAsync(x => x.RequestConcernId == requestConcernIdExist.Id, cancellationToken);

                    bool isChange = false;

                    if (requestConcernIdExist.Concern != command.Concern)
                    {
                        requestConcernIdExist.Concern = command.Concern;
                        isChange = true;
                    }

                    if (isChange)
                    {
                        requestConcernIdExist.ModifiedBy = command.Modified_By;
                        requestConcernIdExist.UpdatedAt = DateTime.Now;
                        ticketConcernExist.UpdatedAt = DateTime.Now;
                        ticketConcernExist.ModifiedBy = command.Modified_By;
                        ticketConcernExist.ConcernDetails = requestConcernIdExist.Concern;
                    }

                    if (ticketConcernExist.IsReject is true)
                    {
                        ticketConcernExist.IsReject = false;
                        ticketConcernExist.Remarks = null;

                        updateRequestList.Add(requestConcernIdExist);
                    }

                    ticketConcernList.Add(ticketConcernExist);

                }
                else
                {

                    var addRequestConcern = new RequestConcern
                    {
                        UserId = userId.Id,
                        Concern = command.Concern,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        IsDone = false

                    };

                    await _context.RequestConcerns.AddAsync(addRequestConcern);
                    await _context.SaveChangesAsync(cancellationToken);


                    var addTicketConcern = new TicketConcern
                    {
                        RequestConcernId = addRequestConcern.Id,
                        RequestorBy = command.UserId,
                        ConcernDetails = addRequestConcern.Concern,
                        IsApprove = false,
                        AddedBy = command.Added_By,
                        TicketType = TicketingConString.Concern,
                        ConcernStatus = addRequestConcern.ConcernStatus,
                        IsAssigned = false,

                    };

                    await _context.TicketConcerns.AddAsync(addTicketConcern);

                    ticketConcernList.Add(addTicketConcern);

                    await _context.SaveChangesAsync(cancellationToken);

                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = addTicketConcern.Id,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Request,
                        Status = $"{TicketingConString.RequestCreated} {userId.Fullname}" 
                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);


                    var userReceiver = await _context.Receivers
                        .FirstOrDefaultAsync(x => x.BusinessUnitId == addRequestConcern.User.BusinessUnitId);


                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"New concern received from : {userDetails.Fullname}",
                        AddedBy = userDetails.Id,
                        Created_At = DateTime.Now,
                        ReceiveBy = userReceiver.UserId.Value,

                    };

                    await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                }


                var uploadTasks = new List<Task>();

                if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.RequestAttachmentsFiles.Where(attachments => attachments.Attachment.Length > 0))
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
                                PublicId = $"MakeITSimple/Ticketing/Request/{userDetails.Fullname}/{attachments.Attachment.FileName}",
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

                                    if (ticketAttachment != null)
                                    {
                                        if (requestConcernIdExist.IsReject is true)
                                        {
                                            updateRequestAttachmentList.Add(ticketAttachment);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                var addAttachment = new TicketAttachment
                                {
                                    TicketConcernId = ticketConcernList.First().Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.TicketAttachments.AddAsync(addAttachment);

                                if (requestConcernIdExist != null)
                                {
                                    if (requestConcernIdExist.IsReject is true)
                                    {
                                        updateRequestAttachmentList.Add(ticketAttachment);
                                    }
                                }


                            }

                        }, cancellationToken));

                    }
                }


                if (updateRequestList.Any() || updateRequestAttachmentList.Any())
                {
                    requestConcernIdExist.Remarks = command.Remarks;
                    requestConcernIdExist.IsReject = false;
                    requestConcernIdExist.RejectBy = null;

                }

                await Task.WhenAll(uploadTasks);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }
    }
}
