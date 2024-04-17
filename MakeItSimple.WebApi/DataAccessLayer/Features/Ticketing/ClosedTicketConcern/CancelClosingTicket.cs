using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class CancelClosingTicket
    {
        public class CancelClosingTicketCommand : IRequest<Result>
        {
            public List<CancelClosingGenerator> CancelClosingGenerators { get; set; }
            public class CancelClosingGenerator
            {
                public int TicketGeneratorId { get; set; }

                public List<CancelClosingConcern> CancelClosingConcerns { get; set; }

                public class CancelClosingConcern
                {
                    public int? ClosingTicketId { get; set; }
                }

            }

        }

        public class Handler : IRequestHandler<CancelClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {

                foreach (var close in command.CancelClosingGenerators)
                {
                    var closingTicketQuery = await _context.ClosingTickets.Where(x => x.TicketGeneratorId == close.TicketGeneratorId).ToListAsync();

                    if (closingTicketQuery == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketIdNotExist());
                    }

                    if (close.CancelClosingConcerns.Count(x => x.ClosingTicketId != null) <= 0)
                    {
                        foreach (var closingList in closingTicketQuery)
                        {
                            var closingTicketConcernRequest = await _context.TicketConcerns.Where(x => x.Id == closingList.TicketConcernId).ToListAsync();

                            foreach (var reTicketConcern in closingTicketConcernRequest)
                            {
                                reTicketConcern.IsClosedApprove = null;
                            }

                            _context.Remove(closingList);
                        }
                    }
                  
                   
                    foreach (var closingId in close.CancelClosingConcerns)
                    {
                        var closingConcernId = closingTicketQuery.FirstOrDefault(x => x.Id == closingId.ClosingTicketId);
                        if (closingConcernId != null)
                        {
                            var reTicketConcernRequest = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == closingConcernId.TicketConcernId, cancellationToken);

                            reTicketConcernRequest.IsClosedApprove = null;
                             
                            _context.Remove(closingConcernId);
                        }
                        else
                        {
                          return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());
                        }

                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
