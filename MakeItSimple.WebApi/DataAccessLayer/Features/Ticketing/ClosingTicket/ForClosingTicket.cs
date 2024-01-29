using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket
{
    public class ForClosingTicket
    {

        public class ForClosingTicketCommand : IRequest<Result>
        {

            public ICollection<ForClosingConcern> ForClosingConcerns { get; set; }
            public class ForClosingConcern
            {
                public int ? TicketConcernId { get; set; }
            }
            
        }

        public class Handler : IRequestHandler<ForClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ForClosingTicketCommand command, CancellationToken cancellationToken)
            {

                var closingGenerator = new ClosingGenerator { };

               await _context.ClosingGenerators.AddAsync(closingGenerator, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach(var closingTicket in command.ForClosingConcerns)
                {
                    
                    var ticketConcern = await _context.TicketConcerns.FirstOrDefaultAsync(x => x.Id == closingTicket.TicketConcernId);

                    if(ticketConcern == null)
                    {
                        return Result.Failure(ClosingTicketError.TicketConcernIdNotExist());
                    }

                    ticketConcern.ClosingGeneratorId = closingGenerator.Id;
                    ticketConcern.IsClosedApprove = false; 

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


        }

    }
}
