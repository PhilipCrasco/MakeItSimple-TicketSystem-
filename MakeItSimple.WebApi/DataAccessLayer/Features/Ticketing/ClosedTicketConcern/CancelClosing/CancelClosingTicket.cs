using Humanizer;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.CancelClosing
{
    public partial class CancelClosingTicket
    {

        public class Handler : IRequestHandler<CancelClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {

                var closingTicketExist = await _context.ClosingTickets
                    .Where(x => x.Id == command.ClosingTicketId)
                    .FirstOrDefaultAsync();

                if (closingTicketExist == null)
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == closingTicketExist.TicketConcernId);

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.ClosingTicketId == command.ClosingTicketId)
                    .ToListAsync();

                foreach (var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id)
                    .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)
                     || x.Request.Contains(TicketingConString.NotConfirm))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

                closingTicketExist.IsActive = false;
                ticketConcernExist.IsClosedApprove = null;  

                await CancelTicketHistory(closingTicketExist, command, cancellationToken); 

                await TransactionHistory(ticketConcernExist,closingTicketExist,command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<TicketHistory> CancelTicketHistory(ClosingTicket closingTicket, CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closingTicket.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Cancel,
                    Status = TicketingConString.CloseCancel,

                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                return addTicketHistory;

            }

            private async Task<TicketTransactionNotification> TransactionHistory(TicketConcern ticketConcern,ClosingTicket closingTicket, CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket closing request #{ticketConcern.Id} has been canceled",
                    AddedBy = closingTicket.AddedBy.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = closingTicket.TicketApprover.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id,
                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;

            }
        }
    }
}
