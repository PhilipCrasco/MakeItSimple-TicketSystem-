using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class CancelTicketRequest 
    {
        public class CancelTicketRequestCommand : IRequest<Result>
        {
            public List<CancelTicketGenerator> CancelTicketGenerators { get; set; }
            public class CancelTicketGenerator
            {
                public int RequestGeneratorId { get; set; }

                public ICollection<CancelTicketConcern> CancelTicketConcerns { get; set; }

                public class CancelTicketConcern
                {
                    public int ? TicketConcernId { get; set; }
                }
            }

        }

        public class Handler : IRequestHandler<CancelTicketRequestCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTicketRequestCommand command, CancellationToken cancellationToken)
            {
                //var cancelList = new List<CancelTicketConcern>();

                foreach (var ticketGenerator in command.CancelTicketGenerators)
                {


                    var requestGeneratorExist = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == ticketGenerator.RequestGeneratorId, cancellationToken);
                    if (requestGeneratorExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == ticketGenerator.RequestGeneratorId).ToListAsync();

                   

                    foreach (var ticketConcern in ticketGenerator.CancelTicketConcerns)
                    {
                        var ticketConcernById = ticketConcernExist.FirstOrDefault(x => x.Id == ticketConcern.TicketConcernId);
                        if(ticketConcernById != null)
                        {
                            ticketConcernById.IsActive = false;
                        }
                        else if(ticketConcern.TicketConcernId == null)
                        {
                            foreach(var cancelAll in ticketConcernExist)
                            {
                                cancelAll.IsActive = false;
                            }
                        }
                        else
                        {
                            return Result.Failure(TicketRequestError.TicketConcernIdNotExist());
                        }

                         
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();    
            }
        }
    }
}
