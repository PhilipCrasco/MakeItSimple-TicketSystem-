using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
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
            public Guid ? IssueHandler { get; set; }
            public string Role { get; set; }

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

        }

        public class Handler : IRequestHandler<AddRequestConcernReceiverCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRequestConcernReceiverCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var requestGeneratorList = new List<RequestGenerator>();

                var ticketConcernList = new List<TicketConcern>();

                var requestGeneratorexist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if(requestGeneratorexist == null)
                {
                    var requestGeneratorId = new RequestGenerator { IsActive = true };
                    await _context.RequestGenerators.AddAsync(requestGeneratorId);
                    await _context.SaveChangesAsync(cancellationToken);
                    requestGeneratorList.Add(requestGeneratorId);
                    
                }

                var requestTicketConcernList = await _context.TicketConcerns.Include(x => x.AddedByUser)
                    .ThenInclude(x => x.UserRole)
                    .Include(x => x.RequestorByUser)
                    .ThenInclude(x => x.UserRole)
                    .Where(x => x.RequestGeneratorId == command.RequestGeneratorId).ToListAsync();

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

                if(command.Role == TicketingConString.IssueHandler)
                {
                    var approverUserValidation = await _context.ApproverTicketings
                        .Where(x => x.RequestGeneratorId == command.RequestGeneratorId && x.IssueHandler == command.Added_By && x.IsApprove != null)
                        .FirstOrDefaultAsync();

                    if (approverUserValidation != null)
                    {
                        var receiverValidation = await _context.TicketConcerns
                            .Where(x => x.RequestGeneratorId == command.RequestGeneratorId && x.UserId == command.Added_By && x.IsApprove != false)
                            .FirstOrDefaultAsync();


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
                    var getApproverUserId = getApproverUser.FirstOrDefault(x => x.ApproverLevel == getApproverUser.Min(x => x.ApproverLevel));

                    var upsertConcern = requestTicketConcernList.FirstOrDefault(x => x.Id == concerns.TicketConcernId);
                    if (upsertConcern != null)
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


                        if (upsertConcern.RequestorByUser.UserRole.UserRoleName == TicketingConString.Requestor 
                            &&   upsertConcern.AddedByUser.UserRole.UserRoleName != TicketingConString.IssueHandler)
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

                        if (command.Role == TicketingConString.IssueHandler)
                        {
                            upsertConcern.IsReject = false;
                            upsertConcern.TicketType = TicketingConString.Concern;
                            upsertConcern.TicketApprover = getApproverUserId.UserId;

                        }
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
                            RequestGeneratorId = requestGeneratorexist.Id == null ? requestGeneratorList.First().Id : requestGeneratorexist.Id,
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


                        if(userList.UserRole.UserRoleName == TicketingConString.Requestor 
                            && addedList.UserRole.UserRoleName != TicketingConString.IssueHandler && concernAlreadyExist == null)
                        {
                            var addRequestConcern = new RequestConcern
                            {

                                RequestGeneratorId = requestGeneratorexist.Id,
                                UserId = addnewTicketConcern.RequestorBy,
                                AddedBy = command.Added_By,
                                ConcernStatus = TicketingConString.ForApprovalTicket,
                                
                                IsDone = false,

                            };

                            await _context.RequestConcerns.AddAsync(addRequestConcern);
                            await _context.SaveChangesAsync(cancellationToken);

                            addnewTicketConcern.TicketType = TicketingConString.Concern;
                            addnewTicketConcern.RequestConcernId = addRequestConcern.Id;
                        }

                        addnewTicketConcern.TicketType = TicketingConString.Concern;

                        if (addedList.UserRole.UserRoleName == TicketingConString.IssueHandler)
                        {

                            addnewTicketConcern.IsReject = false;
                            addnewTicketConcern.TicketType = TicketingConString.Concern;

                            ticketConcernList.Add(addnewTicketConcern);
                        }

                        if (command.Role == TicketingConString.IssueHandler)
                        {
                            addnewTicketConcern.IsReject = false;
                            addnewTicketConcern.TicketApprover = getApproverUserId.UserId;
                        }

                        var checkingList = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.ConcernDetails == addnewTicketConcern.ConcernDetails
                        && x.RequestConcernId != null, cancellationToken);
                        if (checkingList != null)
                        {
                            addnewTicketConcern.RequestConcernId = checkingList.RequestConcernId;
                        }                                                                                                                                                                                                                                                                                                                                                        

                        await _context.TicketConcerns.AddAsync(addnewTicketConcern);

                    }
                        
                }

                if (ticketConcernList.Count() > 0)
                {
                    var getApprover = await _context.Approvers.Include(x => x.User)
                  .Where(x => x.SubUnitId == ticketConcernList.First().User.SubUnitId).ToListAsync();

                    if (getApprover == null)
                    {
                        return Result.Failure(TransferTicketError.NoApproverExist());
                    }


                    
                    foreach (var approver in getApprover)
                    {
                        var approverValidation = await _context.ApproverTicketings
                            .Where(x => x.RequestGeneratorId == requestTicketConcernList.First().RequestGeneratorId 
                            && x.IssueHandler == command.Added_By && x.IsApprove == null)
                            .FirstOrDefaultAsync();

                        if (approverValidation == null)
                        {
                            var addNewApprover = new ApproverTicketing
                            {
                                RequestGeneratorId = requestTicketConcernList.First().RequestGeneratorId,
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
