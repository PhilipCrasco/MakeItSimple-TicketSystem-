using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Migrations;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class AddNewClosingTicket
    {
        public class AddNewClosingTicketCommand : IRequest<Result>
        {
            public string Closed_Remarks { get; set; }
            public Guid? Requestor_By { get; set; }
            public Guid? Added_By { get; set; }

            public List<AddClosingTicketConcern> AddClosingTicketConcerns { get; set; }

            public class AddClosingTicketConcern
            {
                public int TicketConcernId { get; set; }
            }

            public class Handler : IRequestHandler<AddNewClosingTicketCommand, Result>
            {
                private readonly MisDbContext _context;

                public Handler(MisDbContext context)
                {
                    _context = context;
                }

                public async Task<Result> Handle(AddNewClosingTicketCommand command, CancellationToken cancellationToken)
                {

                    var closedList = new List<ClosingTicket>();
                    var ticketTransactionId = new TicketTransaction { IsActive = true };

                    await _context.TicketTransactions.AddAsync(ticketTransactionId, cancellationToken);

                    foreach (var close in command.AddClosingTicketConcerns)
                    {
                        var ticketConcernExist = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.Id == close.TicketConcernId, cancellationToken);

                        if (ticketConcernExist == null)
                        {
                            return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());
                        }

                        var ticketConcernAlreadyExist = await _context.ClosingTickets
                            .FirstOrDefaultAsync(x => x.TicketConcernId == close.TicketConcernId
                        && x.IsClosing != true, cancellationToken);

                        if (ticketConcernAlreadyExist != null)
                        {
                            return Result.Failure(ReTicketConcernError.TicketConcernIdAlreadyExist());
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

                        var approverUser = approverList.First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                        await _context.SaveChangesAsync(cancellationToken);

                        var addNewClosingConcern = new ClosingTicket
                        {
                            TicketConcernId = ticketConcernExist.Id,
                            TicketTransactionId = ticketTransactionId.Id,
                            ChannelId = ticketConcernExist.ChannelId,
                            UserId = ticketConcernExist.UserId,
                            ConcernDetails = ticketConcernExist.ConcernDetails,
                            CategoryId = ticketConcernExist.CategoryId,
                            SubCategoryId = ticketConcernExist.SubCategoryId,
                            StartDate = ticketConcernExist.StartDate,
                            TargetDate = ticketConcernExist.TargetDate,
                            IsClosing = false,
                            TicketApprover = approverUser.UserId,
                            AddedBy = command.Added_By
                        };

                        await _context.ClosingTickets.AddAsync(addNewClosingConcern);

                        ticketConcernExist.IsClosedApprove = false;

                        closedList.Add(addNewClosingConcern);
                         
                    }

                    var getApprover = await _context.Approvers
                    .Where(x => x.SubUnitId == closedList.First().User.SubUnitId).ToListAsync();

                    if (getApprover == null)
                    {
                        return Result.Failure(TransferTicketError.NoApproverExist());
                    }

                    foreach (var approver in getApprover)
                    {
                        var addNewApprover = new ApproverTicketing
                        {
                            TicketTransactionId = ticketTransactionId.Id,
                            //ChannelId = approver.ChannelId,
                            SubUnitId = approver.SubUnitId,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,
                            Status = TicketingConString.CloseTicket,

                        };

                        await _context.ApproverTicketings.AddAsync(addNewApprover, cancellationToken);
                    }

                    var addTicketHistory = new TicketHistory
                    {
                        TicketTransactionId = ticketTransactionId.Id,
                        RequestorBy = command.Requestor_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.CloseTicket,
                        Status = TicketingConString.RequestCreated
                    };

                    await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);

                    return Result.Success();


                }
            }

        }
    }
}
