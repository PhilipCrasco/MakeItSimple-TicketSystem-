using MakeItSimple.WebApi.Common;
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
            public int? RequestGeneratorId { get; set; }
            public int? ChannelId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public Guid? IssueHandler { get; set; }
            public string Role { get; set; }

            public List<AddDevelopingTicketByConcern> AddDevelopingTicketByConcerns { get; set; }
            public class AddDevelopingTicketByConcern
            {
                public int? TicketConcernId { get; set; }
                public Guid? UserId { get; set; }
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

                var requestGeneratorexist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);
                if (requestGeneratorexist == null)
                {
                    var requestGeneratorId = new RequestGenerator { IsActive = true };
                    await _context.RequestGenerators.AddAsync(requestGeneratorId);
                    await _context.SaveChangesAsync(cancellationToken);
                    requestGeneratorList.Add(requestGeneratorId);

                }


                var requestTicketConcernList = await _context.TicketConcerns.Include(x => x.RequestorByUser).ThenInclude(x => x.UserRole)
                    .Where(x => x.RequestGeneratorId == command.RequestGeneratorId).ToListAsync();


                var channelidExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken);
                if (channelidExist == null)
                {
                    return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                var requestoridExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.IssueHandler, cancellationToken);
                if (requestoridExist == null)
                {
                    return Result.Failure(TicketRequestError.UserNotExist());
                }

                foreach(var concerns in command.AddDevelopingTicketByConcerns)
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

                    if (command.AddDevelopingTicketByConcerns.Count(x => x.Concern_Details == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                    && x.SubCategoryId == concerns.SubCategoryId && x.UserId == concerns.UserId) > 1)
                    {
                        return Result.Failure(TicketRequestError.DuplicateConcern());
                    }

                    var getApproverUser = await _context.Approvers.Where(x => x.ChannelId == command.ChannelId).ToListAsync();
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

                        if (hasChanged)
                        {

                            
                        }


                    }


                }





                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }


    }
}
