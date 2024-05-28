using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class CancelTicketConcern
    {
        public class CancelTicketConcernCommand : IRequest<Result>
        {
            public List<CancelRequestTransaction> CancelRequestTransactions { get; set; }
            public class CancelRequestTransaction
            {
                public int RequestTransactionId { get; set; }
                public Guid ? Issue_Handler {  get; set; }
                public ICollection<CancelTicketConcern> CancelTicketConcerns { get; set; }
                public class CancelTicketConcern
                {
                    public int? TicketConcernId { get; set; }
                }
            }

        }

        public class Handler : IRequestHandler<CancelTicketConcernCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTicketConcernCommand command, CancellationToken cancellationToken)
            {
                //var cancelList = new List<CancelTicketConcern>();

                foreach (var requestTransaction in command.CancelRequestTransactions)
                {

                    var requestTransactionExist = await _context.RequestTransactions
                        .FirstOrDefaultAsync(x => x.Id == requestTransaction.RequestTransactionId, cancellationToken);

                    if (requestTransactionExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var issueHandlerExist = await _context.TicketConcerns
                        .Where(x => x.RequestTransactionId == requestTransactionExist.Id
                    && x.UserId == requestTransaction.Issue_Handler)
                        .FirstOrDefaultAsync();

                    if (issueHandlerExist == null)
                    {
                        return Result.Failure(TicketRequestError.UserNotExist());
                    }

                    var ticketConcernExist = await _context.TicketConcerns
                        .Where(x => x.RequestTransactionId == requestTransaction.RequestTransactionId 
                        && x.UserId == requestTransaction.Issue_Handler).ToListAsync();

                    if (requestTransaction.CancelTicketConcerns == null)
                    {
                        foreach (var cancelAll in ticketConcernExist)
                        {
                            cancelAll.IsActive = false;
                        }
                    }

                    foreach (var ticketConcern in requestTransaction.CancelTicketConcerns)
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
