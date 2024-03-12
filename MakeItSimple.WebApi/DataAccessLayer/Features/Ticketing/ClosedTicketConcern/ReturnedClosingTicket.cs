﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ReturnedClosingTicket
    {
        public class ReturnedClosingTicketCommand : IRequest<Result>
        {
            public string Remarks { get; set; }
            public Guid? UserId { get; set; }
            public List<ReturnedClosingTicketById> ReturnedClosingTicketByIds { get; set; }
            public class ReturnedClosingTicketById
            {
                public int ? TicketGeneratorId { get; set; }

            }

        }

        public class Handler : IRequestHandler<ReturnedClosingTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ReturnedClosingTicketCommand command, CancellationToken cancellationToken)
            {

                foreach(var close in command.ReturnedClosingTicketByIds)
                {

                    var requestGeneratorExist = await _context.TicketGenerators.FirstOrDefaultAsync(x => x.Id == close.TicketGeneratorId, cancellationToken);
                    var ticketHistoryList = await _context.TicketHistories.Where(x => x.TicketGeneratorId == requestGeneratorExist.Id).ToListAsync();
                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    var closedList = await _context.ClosingTickets.Where(x => x.TicketGeneratorId == requestGeneratorExist.Id).ToListAsync();
                   

                    foreach(var ticket in closedList)
                    {
                        ticket.Remarks = command.Remarks;
                        ticket.IsRejectClosed = false;
                        ticket.RejectRemarks = null;
                        ticket.RejectClosedBy = null;
                    }

                    if (ticketHistoryId.Status != TicketingConString.Returned)
                    {
                        var addTicketHistory = new TicketHistory
                        {
                            TicketGeneratorId = requestGeneratorExist.Id,
                            RequestorBy = command.UserId,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.CloseTicket,
                            Status = TicketingConString.RejectedBy
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
