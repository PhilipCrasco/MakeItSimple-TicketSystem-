using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Migrations;
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

            public int? RequestGeneratorId { get; set; }
            
            public int ? ChannelId { get; set; }
            public Guid ? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }
            public Guid? IssueHandler { get; set; }
            public string Role { get; set; }

            public string Remarks { get; set; } 

            public List<AddRequestConcernbyConcern> AddRequestConcernbyConcerns { get; set; }
            public class AddRequestConcernbyConcern
            {

                public int ? TicketConcernId { get; set; }
                public Guid ? UserId { get; set; }    
                public string Concern_Details { get; set; }
                public int CategoryId { get; set; }
                public int SubCategoryId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set; }

            }

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

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
            }

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var requestGeneratorList = new List<RequestGenerator>();
                var ticketConcernList = new List<TicketConcern>();
                var removeTicketConcern = new List<TicketConcern>();

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                var requestorPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                var requestGeneratorexist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if (requestGeneratorexist == null)
                {
                    var requestGeneratorId = new RequestGenerator { IsActive = true };
                    await _context.RequestGenerators.AddAsync(requestGeneratorId);
                    requestGeneratorexist = requestGeneratorId;

                }

                await _context.SaveChangesAsync(cancellationToken);
                requestGeneratorList.Add(requestGeneratorexist);

                var requestTicketConcernList = await _context.TicketConcerns.ToListAsync();

                if (command.IssueHandler == null)
                {
                     requestTicketConcernList = await _context.TicketConcerns.Include(x => x.AddedByUser)
                        .ThenInclude(x => x.UserRole)
                        .Include(x => x.RequestorByUser)
                        .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id).ToListAsync();
                }
                else
                {
                     requestTicketConcernList = await _context.TicketConcerns.Include(x => x.AddedByUser)
                        .ThenInclude(x => x.UserRole)
                        .Include(x => x.RequestorByUser)
                        .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id && x.UserId == command.IssueHandler).ToListAsync();
                }

                var channelidExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if(channelidExist == null)
                {
                    return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                var requestoridExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Requestor_By, cancellationToken);
                if (requestoridExist == null)
                {
                    return Result.Failure(TicketRequestError.UserNotExist ());
                }

                if(issueHandlerPermissionList.Any(x => x.Contains(command.Role)))
                {
                    var approverUserList = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id 
                        && x.IssueHandler == command.Added_By && x.ApproverLevel == 1)
                        .ToListAsync();

                    var approverUserValidation = approverUserList
                        .FirstOrDefault(x => x.RequestGeneratorId == requestGeneratorexist.Id && x.IssueHandler == command.Added_By
                        && x.IsApprove != null && x.Id == approverUserList.Max(x => x.Id) && x.ApproverLevel == 1);
                        
                    if (approverUserValidation != null)
                    {

                        var ticketConcernListApprover = await _context.TicketConcerns
                            .Where(x => x.RequestGeneratorId== command.RequestGeneratorId && x.UserId == command.Added_By)
                            .ToListAsync();

                        var receiverValidation = ticketConcernListApprover
                            .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id 
                            && x.UserId == command.Added_By && x.IsApprove != true 
                            && x.Id == ticketConcernListApprover.Max(x => x.Id))
                            .FirstOrDefault();

                        if (receiverValidation != null)
                        {
                            return Result.Failure(TicketRequestError.ConcernWasInApproval());
                        }
                    }
                }

                foreach (var concerns in command.AddRequestConcernbyConcerns)
                {

                    switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == concerns.UserId))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.UserNotExist());
                    }

                    switch (await _context.Categories.FirstOrDefaultAsync(x => x.Id == concerns.CategoryId))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.CategoryNotExist());
                    }
                    switch (await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == concerns.SubCategoryId))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    }

                    if (string.IsNullOrEmpty(concerns.Concern_Details))
                    {
                        return Result.Failure(TicketRequestError.ConcernDetailsNotNull());
                    }

                    if (concerns.Start_Date > concerns.Target_Date || dateToday > concerns.Target_Date)
                    {
                        return Result.Failure(TicketRequestError.DateTimeInvalid());
                    }

                    if (command.AddRequestConcernbyConcerns.Count(x => x.Concern_Details == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                    && x.SubCategoryId == concerns.SubCategoryId && x.UserId == concerns.UserId) > 1)
                    {
                        return Result.Failure(TicketRequestError.DuplicateConcern()); 
                    }

                    var getApproverSubUnit = await _context.Users.FirstOrDefaultAsync(x => x.Id == concerns.UserId, cancellationToken);
                    var getApproverUser = await _context.Approvers.Where(x => x.SubUnitId == getApproverSubUnit.SubUnitId).ToListAsync();
                    if (getApproverUser.Count() < 0)
                    {
                        return Result.Failure(ClosingTicketError.NoApproverHasSetup());
                    }
                    var getApproverUserId = getApproverUser.FirstOrDefault(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel)); 
                    var upsertConcern = requestTicketConcernList
                        .FirstOrDefault(x => x.Id == concerns.TicketConcernId);

                    if (upsertConcern != null )
                    {

                        var duplicateConcern = requestTicketConcernList.FirstOrDefault(x => x.UserId == concerns.UserId && x.ConcernDetails == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                           && x.SubCategoryId == concerns.SubCategoryId && (upsertConcern.ConcernDetails != concerns.Concern_Details
                           && upsertConcern.CategoryId != concerns.CategoryId && upsertConcern.SubCategoryId != concerns.SubCategoryId));

                        if (duplicateConcern != null)
                        {
                            return Result.Failure(TicketRequestError.DuplicateConcern());
                        }

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

                        if (upsertConcern.UserId != concerns.UserId)
                        {
                            upsertConcern.UserId = concerns.UserId;
                            hasChanged = true;
                        }

                        if (upsertConcern.ConcernDetails != concerns.Concern_Details)
                        {
                            upsertConcern.ConcernDetails = concerns.Concern_Details;
                            hasChanged = true;
                        }

                        if (upsertConcern.CategoryId != concerns.CategoryId)
                        {
                            upsertConcern.CategoryId = concerns.CategoryId;
                            hasChanged = true;
                        }

                        if (upsertConcern.SubCategoryId != concerns.SubCategoryId)
                        {
                            upsertConcern.SubCategoryId = concerns.SubCategoryId;
                            hasChanged = true;
                        }

                        if (upsertConcern.StartDate != concerns.Start_Date)
                        {
                            upsertConcern.StartDate = concerns.Start_Date;
                            hasChanged = true;
                        }

                        if (upsertConcern.TargetDate != concerns.Target_Date)
                        {
                            upsertConcern.TargetDate = concerns.Target_Date;
                            hasChanged = true;
                        }


                        if (requestorPermissionList.Any(x => x.Contains(upsertConcern.RequestorByUser.UserRole.UserRoleName))
                            &&  !issueHandlerPermissionList.Any(x => x.Contains(upsertConcern.AddedByUser.UserRole.UserRoleName))) 
                        {
                            var requestUpsertConcern = await _context.RequestConcerns.Where(x => x.Id == upsertConcern.RequestConcernId).ToListAsync();

                            foreach(var request in requestUpsertConcern)
                            {
                                if (request.Concern != upsertConcern.ConcernDetails) 
                                {
                                    request.Concern = upsertConcern.ConcernDetails;
                                    hasChanged = true;
                                    request.ModifiedBy = command.Modified_By;
                                    request.UpdatedAt = DateTime.Now;
                                }
                            }

                        }

                        if (hasChanged)
                        {
                            upsertConcern.ModifiedBy = command.Modified_By;
                            upsertConcern.UpdatedAt = DateTime.Now;
                        }
                        upsertConcern.TicketType = TicketingConString.Concern;

                        if (issueHandlerPermissionList.Any(x => x.Contains(command.Role)))
                        {
                            upsertConcern.TicketType = TicketingConString.Concern;
                            upsertConcern.TicketApprover = getApproverUserId.UserId;

                        }

                        if(upsertConcern.IsReject is true)
                        {
                            upsertConcern.IsReject = false;
                            upsertConcern.Remarks = command.Remarks;
                        }

                        removeTicketConcern.Add(upsertConcern);

                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {

                        var duplicateConcern = requestTicketConcernList.FirstOrDefault(x => x.UserId == concerns.UserId && x.ConcernDetails == concerns.Concern_Details
                        && concerns.CategoryId == concerns.CategoryId && x.SubCategoryId == concerns.SubCategoryId);

                        if (duplicateConcern != null)
                        {
                            return Result.Failure(TicketRequestError.DuplicateConcern());
                        }

                        var addnewTicketConcern = new TicketConcern
                        {
                            RequestGeneratorId = requestGeneratorexist.Id,
                            RequestorBy = command.Requestor_By,
                            ChannelId = command.ChannelId,
                            UserId = concerns.UserId,
                            ConcernDetails = concerns.Concern_Details,
                            CategoryId = concerns.CategoryId,
                            SubCategoryId = concerns.SubCategoryId,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            StartDate = concerns.Start_Date,
                            TargetDate = concerns.Target_Date,
                            IsApprove = false,
                            ConcernStatus = TicketingConString.ForApprovalTicket
                          
                        };

                        var userList = await _context.Users.Include(x => x.UserRole).FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.RequestorBy, cancellationToken);
                        var addedList = await _context.Users.Include(x => x.UserRole).FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.AddedBy, cancellationToken);
                        var concernAlreadyExist = await _context.RequestConcerns.FirstOrDefaultAsync(x => x.Concern == addnewTicketConcern.ConcernDetails && x.IsActive == true, cancellationToken);


                        if(requestorPermissionList.Any(x => x.Contains(userList.UserRole.UserRoleName))
                            && issueHandlerPermissionList.Any(x => !x.Contains(addedList.UserRole.UserRoleName)))
                        {

                            var requestConcernExist = await _context.RequestConcerns
                                .Where(x => x.Concern == addnewTicketConcern.ConcernDetails && x.IsActive)
                                .FirstOrDefaultAsync();

                            if (concernAlreadyExist == null)
                            {
                                var addRequestConcern = new RequestConcern
                                {

                                    RequestGeneratorId = requestGeneratorexist.Id,
                                    UserId = addnewTicketConcern.RequestorBy,
                                    Concern = concerns.Concern_Details,
                                    AddedBy = command.Added_By,
                                    ConcernStatus = TicketingConString.ForApprovalTicket,
                                    IsDone = false,

                                };

                                await _context.RequestConcerns.AddAsync(addRequestConcern);
                                await _context.SaveChangesAsync(cancellationToken);

                                requestConcernExist = addRequestConcern;
                            }

                            addnewTicketConcern.TicketType = TicketingConString.Concern;
                            addnewTicketConcern.RequestConcernId = requestConcernExist.Id;
                        }

                        addnewTicketConcern.TicketType = TicketingConString.Concern;

                        if (issueHandlerPermissionList.Any(x => x.Contains(addedList.UserRole.UserRoleName)))
                        {
                            addnewTicketConcern.TicketType = TicketingConString.Concern;

                            ticketConcernList.Add(addnewTicketConcern);
                        }

                        if (issueHandlerPermissionList.Any(x => x.Contains(command.Role)))
                        {
                            addnewTicketConcern.TicketApprover = getApproverUserId.UserId;
                        }
                        
                        if(requestGeneratorexist != null)
                        {
                            var rejectTicketConcern = await _context.TicketConcerns
                                .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id && x.IsReject == true 
                                && x.UserId == command.Added_By).ToListAsync();

                            foreach(var reject in rejectTicketConcern)
                            {
                                reject.IsReject = false;
                                reject.Remarks = command.Remarks;
                            }

                        }

                        var checkingList = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.ConcernDetails == addnewTicketConcern.ConcernDetails
                        && x.RequestConcernId != null, cancellationToken);
                        if (checkingList != null)
                        {
                            addnewTicketConcern.RequestConcernId = checkingList.RequestConcernId;
                        }                                                                                                                                                                                                                                                                                                                                                        

                        await _context.TicketConcerns.AddAsync(addnewTicketConcern);

                    }

                }

                var selectRemoveConcern = removeTicketConcern.Select(x => x.Id);
                var selectRemoveGenerator = removeTicketConcern.Select(x => x.RequestGeneratorId);

                var removeConcernList = await _context.TicketConcerns
                    .Where(x => !selectRemoveConcern.Contains(x.Id) 
                    && selectRemoveGenerator.Contains(x.RequestGeneratorId) 
                    && x.IsApprove != true && x.IsActive == true)
                    .ToListAsync();

                if (removeConcernList.Count() > 0)
                {
                    foreach (var removeConcern in removeConcernList)
                    {
                        var concernList = await _context.TicketConcerns
                             .Where(x => selectRemoveGenerator.Contains(x.RequestGeneratorId)
                             && x.IsApprove != true && x.IsActive == true)
                             .ToListAsync();

                        if (concernList.Count() > 1)
                        {
                            removeConcern.IsActive = false;
                        }

                        if (concernList.Count() == 1)
                        {
                            removeConcern.UserId = null;
                            removeConcern.CategoryId = null;
                            removeConcern.SubCategoryId = null;
                            removeConcern.StartDate = null;
                            removeConcern.TargetDate = null;
                            removeConcern.UpdatedAt = null;
                            removeConcern.ModifiedBy = null;
                        }
                    }

                }

                var uploadTasks = new List<Task>();

                if (command.ConcernAttachments.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.ConcernAttachments.Where(attachments => attachments.Attachment.Length > 0))
                    {

                        //var ticketAttachmentList = await _context.TicketAttachments.Include(x => x.RequestGenerator)
                        //.Where(x => x.RequestGeneratorId == requestGeneratorList.First().Id).ToListAsync();

                        var ticketAttachment = await _context.TicketAttachments.FirstOrDefaultAsync(x => x.Id == attachments.TicketAttachmentId, cancellationToken);

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
                                PublicId = $"MakeITSimple/Ticketing/Request/{requestGeneratorList.First().Id}/{attachments.Attachment.FileName}"
                            };

                            var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);

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
                                    RequestGeneratorId = requestGeneratorList.First().Id,
                                    Attachment = attachmentResult.SecureUrl.ToString(),
                                    FileName = attachments.Attachment.FileName,
                                    FileSize = attachments.Attachment.Length,
                                    AddedBy = command.Added_By,
                                };

                                await _context.AddAsync(addAttachment, cancellationToken);

                            }

                        }, cancellationToken));

                    }
                }

                await Task.WhenAll(uploadTasks);


                if (ticketConcernList.Count() > 0)
                {
                    var getApprover = await _context.Approvers.Include(x => x.User)
                  .Where(x => x.SubUnitId == ticketConcernList.First().User.SubUnitId).ToListAsync();

                    if (getApprover == null)
                    {
                        return Result.Failure(TransferTicketError.NoApproverExist());
                    }


                    var approverValidation = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id
                        && x.IssueHandler == command.Added_By && x.IsApprove == null)
                        .FirstOrDefaultAsync();

                    if (approverValidation == null)
                    {
                        foreach (var approver in getApprover)
                        {

                            var addNewApprover = new ApproverTicketing
                            {
                                RequestGeneratorId = requestGeneratorexist.Id,
                                //ChannelId = approver.ChannelId,
                                SubUnitId = approver.SubUnitId,
                                UserId = approver.UserId,
                                ApproverLevel = approver.ApproverLevel,
                                AddedBy = command.Added_By,
                                CreatedAt = DateTime.Now,
                                Status = TicketingConString.RequestTicket,

                            };

                            foreach (var issueHandler in ticketConcernList)
                            {
                                addNewApprover.IssueHandler = issueHandler.UserId;
                            }

                            await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken); 
                return Result.Success();
            }
        }
    }
}
