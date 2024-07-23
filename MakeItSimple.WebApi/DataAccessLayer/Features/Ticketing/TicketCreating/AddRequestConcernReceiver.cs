using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddRequestConcernReceiver
    {
        public class AddRequestConcernReceiverCommand : IRequest<Result>
        {

            //public int? RequestTransactionId { get; set; }
            public int? TicketConcernId { get; set; }
            public int ? ChannelId { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Details { get; set; }
            public int CategoryId { get; set; }
            public int SubCategoryId { get; set; }
            public DateTime Start_Date { get; set; }
            public DateTime Target_Date { get; set; }
            public string Role { get; set; }
            public string Remarks { get; set; } 

            public List<ConcernAttachment> ConcernAttachments {  get; set; }

            public class ConcernAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

        }

        public class Handler : IRequestHandler<AddRequestConcernReceiverCommand, Result>
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

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var ticketConcernList = new List<TicketConcern>();
                var removeTicketConcern = new List<TicketConcern>();

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                var allUserList = await _context.UserRoles
                    .ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                var requestorPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                var channelExist = await _context.Channels
                    .FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if(channelExist == null)
                {
                    return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                switch (await _context.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.CategoryNotExist());
                }
                switch (await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == command.SubCategoryId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.SubCategoryNotExist());
                }

                if (string.IsNullOrEmpty(command.Concern_Details))
                {
                    return Result.Failure(TicketRequestError.ConcernDetailsNotNull());
                }

                if (command.Start_Date > command.Target_Date || dateToday > command.Target_Date)
                {
                    return Result.Failure(TicketRequestError.DateTimeInvalid());
                }

                //var duplicateConcern = await _context.TicketConcerns
                //    .Where(x => x.ConcernDetails == command.Concern_Details && x.CategoryId == command.CategoryId 
                //    && x.SubCategoryId == command.SubCategoryId && x.UserId == command.UserId)
                //    .FirstOrDefaultAsync();

                //if (duplicateConcern is not null)
                //{
                //    return Result.Failure(TicketRequestError.DuplicateConcern());
                //}

                var upsertConcern = await _context.TicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TicketConcernId);

                if (upsertConcern != null)
                {

                    bool hasChanged = false;

                    if (upsertConcern.ChannelId != command.ChannelId)
                    {
                        upsertConcern.ChannelId = command.ChannelId;
                        hasChanged = true;
                    }

                    if (upsertConcern.RequestorBy != command.Requestor_By)
                    {
                        upsertConcern.RequestorBy = command.Requestor_By;
                        hasChanged = true;
                    }

                    if (upsertConcern.UserId != command.UserId)
                    {
                        upsertConcern.UserId = command.UserId;
                        hasChanged = true;
                    }

                    if (upsertConcern.ConcernDetails != command.Concern_Details)
                    {
                        upsertConcern.ConcernDetails = command.Concern_Details;
                        hasChanged = true;
                    }

                    if (upsertConcern.CategoryId != command.CategoryId)
                    {
                        upsertConcern.CategoryId = command.CategoryId;
                        hasChanged = true;
                    }

                    if (upsertConcern.SubCategoryId != command.SubCategoryId)
                    {
                        upsertConcern.SubCategoryId = command.SubCategoryId;
                        hasChanged = true;
                    }

                    if (upsertConcern.StartDate != command.Start_Date)
                    {
                        upsertConcern.StartDate = command.Start_Date;
                        hasChanged = true;
                    }

                    if (upsertConcern.TargetDate != command.Target_Date)
                    {
                        upsertConcern.TargetDate = command.Target_Date;
                        hasChanged = true;
                    }

                    if (upsertConcern.RequestConcernId != null)
                    {
                        var requestConcern = await _context.RequestConcerns
                            .FirstOrDefaultAsync(x => x.Id == upsertConcern.RequestConcernId, cancellationToken);

                        requestConcern.Remarks = null;
                    }

                    //if(upsertConcern.IsTransfer is true)
                    //{
                    //    upsertConcern.IsTransfer = null;
                    //    upsertConcern.TransferAt = null;
                    //    upsertConcern.TransferBy =  null;
                    //    upsertConcern.Remarks = null;

                    //}

                    var requestUpsertConcern = await _context.RequestConcerns
                        .Where(x => x.Id == upsertConcern.RequestConcernId)
                         .ToListAsync();

                    foreach (var request in requestUpsertConcern)
                    {
                        if (request.Concern != upsertConcern.ConcernDetails)
                        {
                            request.Concern = upsertConcern.ConcernDetails;
                            hasChanged = true;
                            request.ModifiedBy = command.Modified_By;
                            request.UpdatedAt = DateTime.Now;
                            if(request.IsReject is true)
                            {
                                request.Remarks = null;
                                request.IsReject = false;
                            }

                        }
                    }

                    if (hasChanged)
                    {
                        upsertConcern.ModifiedBy = command.Modified_By;
                        upsertConcern.UpdatedAt = DateTime.Now;
                        upsertConcern.IsAssigned = true;
                         
                        if (upsertConcern.IsReject is true)
                        {
                            upsertConcern.IsReject = false;
                            upsertConcern.Remarks = null;
                        }

                    }


                    upsertConcern.TicketType = TicketingConString.Concern; 

                    removeTicketConcern.Add(upsertConcern);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {

                    var addnewTicketConcern = new TicketConcern
                    {
                        RequestorBy = command.Requestor_By,
                        ChannelId = command.ChannelId,
                        UserId = command.UserId,
                        ConcernDetails = command.Concern_Details,
                        CategoryId = command.CategoryId,
                        SubCategoryId =command.SubCategoryId,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                        StartDate = command.Start_Date,
                        TargetDate = command.Target_Date,                      
                        IsApprove = false,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        IsAssigned = true,
                        TicketType = TicketingConString.Concern
                    };


                    await _context.TicketConcerns.AddAsync(addnewTicketConcern);
                    await _context.SaveChangesAsync(cancellationToken);

                    upsertConcern = addnewTicketConcern;

                   var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = addnewTicketConcern.Id,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Request,
                        Status = $"{TicketingConString.RequestCreated} {userDetails.Fullname}"
                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    var addRequestConcern = new RequestConcern
                    {

                        UserId = addnewTicketConcern.RequestorBy,
                        Concern = command.Concern_Details,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        IsDone = false,

                    };

                    await _context.RequestConcerns.AddAsync(addRequestConcern);
                    await _context.SaveChangesAsync(cancellationToken);

                    addnewTicketConcern.RequestConcernId = addRequestConcern.Id;

                    if (receiverPermissionList.Any(x => x.Contains(command.Role)))
                    {
                        addnewTicketConcern.IsApprove = true;

                        var issueHandlerDetails = await _context.Users
                            .FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.UserId);

                        var addAssignHistory = new TicketHistory
                        {
                            TicketConcernId = addnewTicketConcern.Id,
                            TransactedBy = command.UserId,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ConcernAssign,
                            Status = $"{TicketingConString.RequestAssign} {issueHandlerDetails.Fullname}"
                        };

                        await _context.TicketHistories.AddAsync(addAssignHistory, cancellationToken);
                    }

                }

                var uploadTasks = new List<Task>();

                if (command.ConcernAttachments.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.ConcernAttachments.Where(attachments => attachments.Attachment.Length > 0))
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
                                PublicId = $"MakeITSimple/Ticketing/Request/{userDetails.Fullname}/{attachments.Attachment.FileName}"
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
                                    TicketConcernId = upsertConcern.Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.TicketAttachments.AddAsync(addAttachment, cancellationToken);

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
