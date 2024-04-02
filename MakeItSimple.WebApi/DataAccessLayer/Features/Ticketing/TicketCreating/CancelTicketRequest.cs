using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelTicketRequest
    {
        public class CancelTicketRequestCommand : IRequest<Result>
        {
            public List<CancelTicketGenerator> CancelTicketGenerators { get; set; }
            public class CancelTicketGenerator
            {
                public int RequestGeneratorId { get; set; }
                public Guid ? Issue_Handler {  get; set; }
                public ICollection<CancelTicketConcern> CancelTicketConcerns { get; set; }
                public class CancelTicketConcern
                {
                    public int? TicketConcernId { get; set; }
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

                    var issueHandlerExist = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == requestGeneratorExist.Id
                    && x.UserId == ticketGenerator.Issue_Handler).FirstOrDefaultAsync();

                    if (issueHandlerExist == null)
                    {
                        return Result.Failure(TicketRequestError.UserNotExist());
                    }
                    var ticketConcernExist = await _context.TicketConcerns
                        .Where(x => x.RequestGeneratorId == ticketGenerator.RequestGeneratorId && x.UserId == ticketGenerator.Issue_Handler).ToListAsync();

                    if (ticketGenerator.CancelTicketConcerns == null)
                    {
                        foreach (var cancelAll in ticketConcernExist)
                        {
                            cancelAll.IsActive = false;
                        }
                    }

                    foreach (var ticketConcern in ticketGenerator.CancelTicketConcerns)
                    {
                        var ticketConcernById = ticketConcernExist.FirstOrDefault(x => x.Id == ticketConcern.TicketConcernId);
                        if (ticketConcernById != null)
                        {
                            ticketConcernById.IsActive = false;
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
