using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            public Guid? IssueHandler { get; set; }
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
        }


        public class Handler : IRequestHandler<AddDevelopingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddDevelopingTicketCommand command, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Today;

                var requestGeneratorList = new List<RequestGenerator>();

                var ticketConcernList = new List<TicketConcern>();

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

                if (command.Role == TicketingConString.IssueHandler)
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

                    var upsertConcern = requestTicketConcernList.FirstOrDefault(x => x.Id == concerns.TicketConcernId);
                    if(upsertConcern != null)
                    {

                        var duplicateConcern = requestTicketConcernList.FirstOrDefault(x => x.ConcernDetails == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
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
                            upsertConcern.TicketType = TicketingConString.Concern;
                        }

                        if (upsertConcern.IsReject is true)
                        {
                            upsertConcern.IsReject = false;
                            upsertConcern.Remarks = command.Remarks;
                        }

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
                            IsApprove = false

                        };

                        var userList = await _context.Users.Include(x => x.UserRole)
                            .FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.RequestorBy, cancellationToken);

                        var addedList = await _context.Users.Include(x => x.UserRole)
                            .FirstOrDefaultAsync(x => x.Id == addnewTicketConcern.AddedBy, cancellationToken);


                        addnewTicketConcern.TicketType = TicketingConString.Manual;

                        if (addedList.UserRole.UserRoleName == TicketingConString.IssueHandler)
                        {

                            addnewTicketConcern.IsReject = false;

                            if(command.RequestGeneratorId == null)
                            {
                                ticketConcernList.Add(addnewTicketConcern);
                            }

                        }

                        if (command.Role == TicketingConString.IssueHandler)
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
