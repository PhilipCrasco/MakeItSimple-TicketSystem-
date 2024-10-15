using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public partial class CancelTransferTicket
    {

        public class Handler : IRequestHandler<CancelTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var transferTicketExist = await _context.TransferTicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId);

                if (transferTicketExist == null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                

                transferTicketExist.IsActive = false;

                var ticketConcernExist  = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == transferTicketExist.TicketConcernId);

                ticketConcernExist.IsTransfer = null;

                var approverList = await _context.ApproverTicketings
                    .Where(x =>  x.TransferTicketConcernId == command.TransferTicketId)
                    .ToListAsync();

                foreach(var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => (x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval)))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

                await CreateTicketHistory(ticketConcernExist, command, cancellationToken);

                await CreateTransactionNotification(ticketConcernExist,transferTicketExist,command,cancellationToken);


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task CreateTicketHistory(TicketConcern ticketConcern, CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcern.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Cancel,
                    Status = TicketingConString.TransferCancel
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

            }

            private async Task CreateTransactionNotification(TicketConcern ticketConcern, TransferTicketConcern transferTicketConcern, CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket transfer request #{ticketConcern.Id} has been canceled",
                    AddedBy = transferTicketConcern.AddedBy.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TicketApprover.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = ticketConcern.Id

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);
            }


        }
    }
}
