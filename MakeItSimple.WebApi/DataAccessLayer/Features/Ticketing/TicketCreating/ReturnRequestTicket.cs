using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class ReturnRequestTicket
    {
        public class ReturnRequestTicketCommand : IRequest<Result>
        {

            public string Remarks { get; set; }
            public List<ReturnTicketRequestById> ReturnTicketRequestByIds {  get; set; }

            public class ReturnTicketRequestById
            {
                public int? RequestTransactionId { get; set; }
                public Guid ? Issue_Handler {  get; set; }
            }

        }

        public class Handler : IRequestHandler<ReturnRequestTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<Result> Handle(ReturnRequestTicketCommand command, CancellationToken cancellationToken)
            {
                foreach (var ticketConcern in command.ReturnTicketRequestByIds)
                {
                    var requestTransactionExist = await _context.RequestTransactions
                        .FirstOrDefaultAsync(x => x.Id == ticketConcern.RequestTransactionId, cancellationToken);

                    if (requestTransactionExist == null)
                    {
                        return Result.Failure(TicketRequestError.TicketIdNotExist());
                    }

                    var issueHandlerExist = await _context.TicketConcerns
                        .Where(x => x.RequestTransactionId == requestTransactionExist.Id
                    && x.UserId == ticketConcern.Issue_Handler)
                        .FirstOrDefaultAsync();

                    if (issueHandlerExist == null)
                    {
                        return Result.Failure(TicketRequestError.UserNotExist());   
                    }

                    var ticketConcernExist = await _context.TicketConcerns
                        .Where(x => x.RequestTransactionId == ticketConcern.RequestTransactionId && x.UserId == ticketConcern.Issue_Handler).ToListAsync();

                    foreach (var concerns in ticketConcernExist)
                    {
                        concerns.IsReject = false;
                        concerns.IsApprove = false;
                        concerns.Remarks = command.Remarks;
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
