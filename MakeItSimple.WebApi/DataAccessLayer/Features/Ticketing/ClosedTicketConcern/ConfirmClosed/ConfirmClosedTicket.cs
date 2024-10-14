using Azure.Core;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ConfirmClosed
{
    public partial class ConfirmClosedTicket
    {

        public class Hanler : IRequestHandler<ConfirmClosedTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Hanler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ConfirmClosedTicketCommand command, CancellationToken cancellationToken)
            {

                var requestConcernId = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.RequestConcernId, cancellationToken);

                var updateRequestConcern = await UpdateConfirmTicket(requestConcernId,command, cancellationToken);
                if (updateRequestConcern is not null)
                    return updateRequestConcern;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.RequestConcernId == command.RequestConcernId, cancellationToken);

                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                    .FirstOrDefaultAsync();

                if (ticketHistory is not null)
                    await UpdateTicketHistory(ticketHistory,ticketConcernExist,command, cancellationToken); 
               
                await TransactionNotification(ticketHistory,requestConcernId,ticketConcernExist,command,cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task<Result?> UpdateConfirmTicket(RequestConcern requestConcern, ConfirmClosedTicketCommand command, CancellationToken cancellationToken)
            {

                if (requestConcern is null)
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());


                if (requestConcern.Is_Confirm is true)
                    return Result.Failure(TicketRequestError.ConfirmAlready());


                requestConcern.Is_Confirm = true;
                requestConcern.Confirm_At = DateTime.Today;
                requestConcern.ConcernStatus = TicketingConString.Done;

                return null;

            }
            
            private async Task<TicketHistory> UpdateTicketHistory(TicketHistory ticketHistory,TicketConcern ticketConcern, ConfirmClosedTicketCommand command , CancellationToken cancellationToken)
            {
                ticketHistory.TicketConcernId = ticketConcern.Id;
                ticketHistory.TransactedBy = command.Transacted_By;
                ticketHistory.TransactionDate = DateTime.Now;
                ticketHistory.Request = TicketingConString.Confirm;
                ticketHistory.Status = TicketingConString.CloseConfirm;

                return ticketHistory;   
            }

            private async Task<TicketTransactionNotification> TransactionNotification(TicketHistory ticketHistory,RequestConcern requestConcern,TicketConcern ticketConcern, ConfirmClosedTicketCommand command, CancellationToken cancellationToken)
            {

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcern.Id} has been closed",

                    AddedBy = command.Transacted_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = requestConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.Closed,
                    PathId = ticketConcern.Id

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;

            }


        }
    }
}
