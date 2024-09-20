using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class ClickedTransaction
    {
        public class ClickedTransactionCommand : IRequest<Result>
        {
            public int Id { get; set; } 

        }

        public class Handler : IRequestHandler<ClickedTransactionCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async  Task<Result> Handle(ClickedTransactionCommand command, CancellationToken cancellationToken)
            {
                var transactionExist = await _context.TicketTransactionNotifications
                    .FirstOrDefaultAsync(x => x.Id == command.Id);

                if(transactionExist is null)
                {
                    return Result.Failure(TicketRequestError.TransactionNotExist());
                }

                transactionExist.IsChecked = true;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
