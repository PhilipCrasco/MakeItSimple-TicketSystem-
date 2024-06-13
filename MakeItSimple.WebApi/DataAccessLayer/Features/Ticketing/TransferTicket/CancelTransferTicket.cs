using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class CancelTransferTicket
    {
        public class CancelTransferTicketCommand : IRequest<Result>
        {

            public int TransferTicketId { get; set; }
            public Guid ? Transacted_By {  get; set; }


        }

        public class Handler : IRequestHandler<CancelTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelTransferTicketCommand command, CancellationToken cancellationToken)
            {


                var transferTicketExist = await _context.TransferTicketConcerns
                .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId);

                if (transferTicketExist == null)
                {
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                }

                transferTicketExist.IsActive = false;

                var ticketConcernExist  = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == transferTicketExist.TicketConcernId);

                ticketConcernExist.IsTransfer = null;

                var approverList = await _context.ApproverTicketings
                    .Where(x =>  x.TransferTicketConcernId == command.TransferTicketId)
                    .ToListAsync();

                foreach(var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }
                 
                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcernExist.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Transfer,
                    Status = TicketingConString.TransferCancel
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
