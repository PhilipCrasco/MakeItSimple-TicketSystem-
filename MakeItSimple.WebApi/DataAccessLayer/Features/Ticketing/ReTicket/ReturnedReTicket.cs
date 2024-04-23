using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket
{
    public class ReturnedReTicket
    {
        public class ReturnedReTicketCommand : IRequest<Result>
        {
            public string Remarks { get; set; }
            public Guid? UserId { get; set; }
            public List<ReTicket> ReTickets { get; set; }
            public class ReTicket
            {
                public int ? TicketGeneratorId { get; set; }

            }
        }


        public class Handler : IRequestHandler<ReturnedReTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ReturnedReTicketCommand command, CancellationToken cancellationToken)
            {

                foreach(var reTicket in command.ReTickets)
                {

                    var reTicketExist = await _context.TicketGenerators
                        .Where(x => x.Id == reTicket.TicketGeneratorId).FirstOrDefaultAsync();
                    if (reTicketExist == null)
                    {
                        return Result.Failure(ReTicketConcernError.TicketIdNotExist());
                    }

                    var ticketHistoryList = await _context.TicketHistories.Where(x => x.TicketGeneratorId == reTicketExist.Id).ToListAsync();
                    var ticketHistoryId = ticketHistoryList.FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                    var reticketList = await _context.ReTicketConcerns.Where(x => x.TicketGeneratorId == reTicket.TicketGeneratorId).ToListAsync();

                    foreach (var ticket in reticketList)
                    {
                        ticket.Remarks = command.Remarks;
                        ticket.IsRejectReTicket = false;
                        ticket.RejectRemarks = null;
                        ticket.RejectReTicketBy = null;
                    }

                    if (ticketHistoryId.Status != TicketingConString.Returned)
                    {
                        var addTicketHistory = new TicketHistory
                        {
                            TicketGeneratorId = reTicketExist.Id,
                            RequestorBy = command.UserId,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ReTicket,
                            Status = TicketingConString.Returned
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);
                    }
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }
        }
    }
}
