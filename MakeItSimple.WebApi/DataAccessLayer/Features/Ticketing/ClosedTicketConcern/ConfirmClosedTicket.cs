﻿using Azure.Core;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ConfirmClosedTicket
    {
        public class ConfirmClosedTicketCommand : IRequest<Result>
        {
            public int? RequestConcernId { get; set; }
            public Guid ? Transacted_By { get; set; }   
            public string Modules { get; set; }

        }

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
                
                if (requestConcernId is null)
                {
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());
                }
                if (requestConcernId.Is_Confirm is true)
                {
                    return Result.Failure(TicketRequestError.ConfirmAlready());
                }

                requestConcernId.Is_Confirm = true; 
                requestConcernId.Confirm_At = DateTime.Today;
                requestConcernId.ConcernStatus = TicketingConString.Done;
                
                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.RequestConcernId == command.RequestConcernId, cancellationToken);


                var ticketHistory = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                    .FirstOrDefaultAsync();

                if (ticketHistory is not null) 
                {
                    ticketHistory.TicketConcernId = ticketConcernExist.Id;
                    ticketHistory.TransactedBy = command.Transacted_By;
                    ticketHistory.TransactionDate = DateTime.Now;
                    ticketHistory.Request = TicketingConString.Confirm;
                    ticketHistory.Status = TicketingConString.CloseConfirm;

                }

                //var addTicketHistory = new TicketHistory
                //{
                //    TicketConcernId = ticketConcernExist.Id,
                //    TransactedBy = command.Transacted_By,
                //    TransactionDate = DateTime.Now,
                //    Request = TicketingConString.Confirm,
                //    Status = TicketingConString.CloseConfirm,
                //};

                //await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcernExist.Id} has been closed",
                    AddedBy = command.Transacted_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = requestConcernId.UserId.Value,
                    Modules = command.Modules,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
