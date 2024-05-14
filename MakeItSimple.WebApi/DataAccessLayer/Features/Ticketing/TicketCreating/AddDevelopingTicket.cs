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

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class AddDevelopingTicket
    {
        public class AddDevelopingTicketCommand : IRequest<Result>
        {
            public int ? RequestGeneratorId { get; set; }
            public int? ChannelId { get; set; }
            public Guid? UserId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public string Role { get; set; }
            public string Remarks { get; set; }

            public List<AddDevelopingTicketByConcern> AddDevelopingTicketByConcerns { get; set; }
            public class AddDevelopingTicketByConcern
            {
                public int? TicketConcernId { get; set; }
                public string Concern_Details { get; set; }
                public int CategoryId { get; set; }
                public int SubCategoryId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set; }
            }

            public List<ManualAttachment> ManualAttachments { get; set; }
            public class ManualAttachment 
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }


        }


        public class Handler : IRequestHandler<AddDevelopingTicketCommand, Result>
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

            public async Task<Result> Handle(AddDevelopingTicketCommand command, CancellationToken cancellationToken)
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

                var requestTicketConcernList = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .ThenInclude(x => x.UserRole)
                    .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id).ToListAsync();


                var channelidExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if (channelidExist == null)
                {
                    return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                if (issueHandlerPermissionList.Any(x => x.Contains(command.Role)))
                {
                    var approverUserList = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == command.RequestGeneratorId && x.IssueHandler == command.Added_By && x.ApproverLevel == 1).ToListAsync();

                    var approverUserValidation = approverUserList
                        .FirstOrDefault(x => x.RequestGeneratorId == command.RequestGeneratorId && x.IssueHandler == command.Added_By
                        && x.IsApprove != null && x.Id == approverUserList.Max(x => x.Id) && x.ApproverLevel == 1);


                    if (approverUserValidation != null)
                    {

                        var ticketConcernListApprover = await _context.TicketConcerns.Where(x => x.RequestGeneratorId
                        == command.RequestGeneratorId && x.UserId == command.Added_By).ToListAsync();

                        var receiverValidation = ticketConcernListApprover 
                            .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id
                            && x.UserId == command.Added_By && x.IsApprove != true && x.Id == ticketConcernListApprover.Max(x => x.Id))
                            .FirstOrDefault();

                        if (receiverValidation != null)
                        {
                            return Result.Failure(TicketRequestError.ConcernWasInApproval());
                        }

                    }
                }

                foreach (var concerns in command.AddDevelopingTicketByConcerns)
                {


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

                    if (command.AddDevelopingTicketByConcerns.Count(x => x.Concern_Details == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                    && x.SubCategoryId == concerns.SubCategoryId) > 1)
                    {
                        return Result.Failure(TicketRequestError.DuplicateConcern());
                    }

                    var getApproverSubUnit = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);
                    var getApproverUser = await _context.Approvers.Where(x => x.SubUnitId == getApproverSubUnit.SubUnitId).ToListAsync();
                    var getApproverUserId = getApproverUser.FirstOrDefault(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel));

                    var upsertConcern = requestTicketConcernList
                        .FirstOrDefault(x => x.Id == concerns.TicketConcernId);

                    if(upsertConcern != null)
                    {

                        var duplicateConcern = requestTicketConcernList.FirstOrDefault(x => x.ConcernDetails == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                           && x.SubCategoryId == concerns.SubCategoryId && (upsertConcern.ConcernDetails != concerns.Concern_Details
                           && upsertConcern.CategoryId != concerns.CategoryId && upsertConcern.SubCategoryId != concerns.SubCategoryId));

                        if (duplicateConcern != null)
                        {
                            return Result.Failure(TicketRequestError.DuplicateConcern());
                        }

                        //if(command.Role == TicketingConString.IssueHandler && c)

                        bool hasChanged = false;

                        if (upsertConcern.ChannelId != command.ChannelId)
                        {
                            upsertConcern.ChannelId = command.ChannelId;
                            hasChanged = true;
                        }

                        if (upsertConcern.UserId != command.UserId)
                        {
                            upsertConcern.UserId = command.UserId;
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

                        if (hasChanged)
                        {
                            upsertConcern.ModifiedBy = command.Modified_By;
                            upsertConcern.UpdatedAt = DateTime.Now;
                            upsertConcern.TicketType = TicketingConString.Manual;
                        }

                        if (upsertConcern.IsReject is true)
                        {
                            upsertConcern.IsReject = false;
                            upsertConcern.Remarks = command.Remarks;
                        }

                        removeTicketConcern.Add(upsertConcern);

                    }
                    else
                    {
                        var duplicateConcern = requestTicketConcernList.FirstOrDefault(x => x.ConcernDetails == concerns.Concern_Details
                        && concerns.CategoryId == concerns.CategoryId && x.SubCategoryId == concerns.SubCategoryId);

                        if (duplicateConcern != null)
                        {
                            return Result.Failure(TicketRequestError.DuplicateConcern());
                        }


                        var addnewTicketConcern = new TicketConcern
                        {
                            RequestGeneratorId = requestGeneratorexist == null ? requestGeneratorList.First().Id : requestGeneratorexist.Id,
                            RequestorBy = command.UserId,
                            ChannelId = command.ChannelId,
                            UserId = command.UserId,
                            ConcernDetails = concerns.Concern_Details,
                            CategoryId = concerns.CategoryId,
                            SubCategoryId = concerns.SubCategoryId,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            StartDate = concerns.Start_Date,
                            TargetDate = concerns.Target_Date,
                            IsApprove = false,


                        };

                        var userList = await _context.Users.Include(x => x.UserRole)
                            .FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.RequestorBy, cancellationToken);

                        var addedList = await _context.Users.Include(x => x.UserRole)
                            .FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.AddedBy, cancellationToken);


                        addnewTicketConcern.TicketType = TicketingConString.Manual;

                        if (issueHandlerPermissionList.Any(x => x.Contains(addedList.UserRole.UserRoleName)))
                        {

                            addnewTicketConcern.IsReject = false;

                            if(command.RequestGeneratorId == null)
                            {
                                ticketConcernList.Add(addnewTicketConcern);
                            }

                        }

                        if (issueHandlerPermissionList.Any(x => x.Contains(command.Role)))
                        {
                            addnewTicketConcern.TicketApprover = getApproverUserId.UserId;
                        }

                        if (requestGeneratorexist != null)
                        {
                            var rejectTicketConcern = await _context.TicketConcerns
                                .Where(x => x.RequestGeneratorId == requestGeneratorexist.Id && x.IsReject == true
                                && x.UserId == command.Added_By).ToListAsync();

                            foreach (var reject in rejectTicketConcern)
                            {
                                reject.IsReject = false;
                                reject.Remarks = command.Remarks;
                            }

                        }

                        await _context.TicketConcerns.AddAsync(addnewTicketConcern);
                        await _context.SaveChangesAsync(cancellationToken);

                    }
                }

                var selectRemoveConcern = removeTicketConcern.Select(x => x.Id);
                var selectRemoveGenerator = removeTicketConcern.Select(x => x.RequestGeneratorId);
                var selectRemoveUserId = removeTicketConcern.Select(x => x.UserId);
                
                var removeConcernList = await _context.TicketConcerns
                    .Where(x => !selectRemoveConcern.Contains(x.Id)
                    && selectRemoveGenerator.Contains(x.RequestGeneratorId)
                    && selectRemoveUserId.Contains(x.UserId)
                    && x.IsApprove != true && x.IsActive == true)
                    .ToListAsync();

                if (removeConcernList.Count() > 0)
                {
                    foreach (var removeConcern in removeConcernList)
                    {
                        removeConcern.IsActive = false;
                    }
                }

                    var uploadTasks = new List<Task>();

                if (command.ManualAttachments.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.ManualAttachments.Where(attachments => attachments.Attachment.Length > 0))
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

                        //var userAttachment = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Added_By, cancellationToken);

                        uploadTasks.Add(Task.Run(async () =>
                        {
                            await using var stream = attachments.Attachment.OpenReadStream();

                            var attachmentsParams = new RawUploadParams
                            {
                                File = new FileDescription(attachments.Attachment.FileName, stream),
                                PublicId = $"MakeITSimple/Ticketing/Request/{requestGeneratorexist.Id}/{attachments.Attachment.FileName}"
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
                        }
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }


    }
}
