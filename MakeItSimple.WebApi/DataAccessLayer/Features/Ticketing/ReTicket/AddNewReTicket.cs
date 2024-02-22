using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Experimental;
using NuGet.LibraryModel;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class AddNewReTicket
    {
        public class AddNewReTicketCommand : IRequest<Result>
        {
            public string Re_Ticket_Remarks { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid ? Added_By { get; set; }
            public List<AddReTicketConcern> AddReTicketConcerns { get; set; }
            public class AddReTicketConcern
            {
                public int TicketConcernId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set;}
            }
        }

        public class Handler : IRequestHandler<AddNewReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewReTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var reTicketList = new List<ReTicketConcern>();
                var requestGeneratorId = new RequestGenerator { IsActive = true };

                await _context.RequestGenerators.AddAsync(requestGeneratorId, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach(var reTicket in command.AddReTicketConcerns)
                {
                   var ticketConcernExist = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == reTicket.TicketConcernId, cancellationToken);

                   if(ticketConcernExist == null)
                    {
                            return Result.Failure(ReTicketConcernError.TicketConcernIdNotExist());
                    }

                    var ticketConcernAlreadyExist = await _context.ReTicketConcerns.FirstOrDefaultAsync(x => x.TicketConcernId == reTicket.TicketConcernId 
                    && x.IsReTicket != true, cancellationToken);

                    if (ticketConcernAlreadyExist != null)
                    {
                        return Result.Failure(ReTicketConcernError.TicketConcernIdAlreadyExist());
                    }

                    if(reTicket.Start_Date > reTicket.Target_Date || dateToday > reTicket.Target_Date)
                    {
                       return Result.Failure(ReTicketConcernError.DateTimeInvalid());
                    }

                    var approverList = await _context.Approvers.Where(x => x.ChannelId == ticketConcernExist.ChannelId).ToListAsync();

                    var approverUser = approverList.First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    var addNewReTicketConcern = new ReTicketConcern
                    {
                        RequestGeneratorId = requestGeneratorId.Id,
                        TicketConcernId = ticketConcernExist.Id,
                        DepartmentId = ticketConcernExist.DepartmentId,
                        SubUnitId = ticketConcernExist.SubUnitId,
                        ChannelId = ticketConcernExist.ChannelId,
                        UserId = ticketConcernExist.UserId,
                        ConcernDetails = ticketConcernExist.ConcernDetails,
                        CategoryId = ticketConcernExist.CategoryId,
                        SubCategoryId = ticketConcernExist.SubCategoryId,
                        ReTicketRemarks = command.Re_Ticket_Remarks,
                        StartDate = reTicket.Start_Date,
                        TargetDate = reTicket.Target_Date,
                        IsReTicket = false,
                        TicketApprover = approverUser.UserId,
                        AddedBy = command.Added_By
                        
                    };

                    await _context.ReTicketConcerns.AddAsync(addNewReTicketConcern);

                    ticketConcernExist.IsReTicket = false;

                    reTicketList.Add(addNewReTicketConcern);
                }

                var getApprover = await _context.Approvers
                .Where(x => x.ChannelId == reTicketList.First().ChannelId).ToListAsync();

                if (getApprover == null)
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                foreach (var approver in getApprover)
                {
                    var addNewApprover = new ApproverTicketing
                    {
                        RequestGeneratorId = requestGeneratorId.Id,
                        ChannelId = approver.ChannelId,
                        UserId = approver.UserId,
                        ApproverLevel = approver.ApproverLevel,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                        Status = TicketingConString.ReTicket,

                    };

                    await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                }

                var addTicketHistory = new TicketHistory
                {
                    RequestGeneratorId = requestGeneratorId.Id,
                    RequestorBy = command.Requestor_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ReTicket,
                    Status = TicketingConString.RequestCreated
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
