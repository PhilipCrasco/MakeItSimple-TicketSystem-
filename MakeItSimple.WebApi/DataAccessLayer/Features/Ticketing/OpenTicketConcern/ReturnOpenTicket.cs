using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern
{
    public class ReturnOpenTicket
    {

        public class ReturnOpenTicketCommand : IRequest<Result>
        {
            public int? RequestTransactionId { get; set; }
            public string Remarks { get; set; }
        }

        public class Handler : IRequestHandler<ReturnOpenTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ReturnOpenTicketCommand command, CancellationToken cancellationToken)
            {
                var requestTransactionExist = await _context.TicketConcerns
                    .Where(x => x.Id == command.RequestTransactionId)
                    .ToListAsync();

                var requestTicket = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == command.RequestTransactionId);

                if (requestTransactionExist is null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());
                }

                if(requestTransactionExist.Count() > 1)
                {
                    return Result.Failure(TicketRequestError.InvalidReturnTicket());
                }
                
                requestTicket.Remarks = command.Remarks;

                foreach(var transaction in requestTransactionExist)
                {
                    transaction.Remarks = command.Remarks;
                    transaction.IsApprove = false;
                    transaction.IsAssigned = false;
                    transaction.UserId = null;
                    transaction.ChannelId = null;

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }


    
}
