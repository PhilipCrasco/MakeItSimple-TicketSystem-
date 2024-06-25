using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate
{
    public class CancelReDate
    {
        public class CancelReDateCommand : IRequest<Result>
        {
            public int TicketReDateId { get; set; }
            public Guid? Transacted_By { get; set; }

        }


        public class Handler : IRequestHandler<CancelReDateCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelReDateCommand command, CancellationToken cancellationToken)
            {

                var reDateExist = await _context.TicketReDates
                .FirstOrDefaultAsync(x => x.Id == command.TicketReDateId,cancellationToken);

                if (reDateExist == null)
                {
                    return Result.Failure(ReDateError.ReDateIdNotExist());
                }

                reDateExist.IsActive = false;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == reDateExist.TicketConcernId);

                ticketConcernExist.IsReDate = null;

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.TicketReDateId == command.TicketReDateId)
                    .ToListAsync();

                foreach (var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = ticketConcernExist.Id,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ForReDate,
                    Status = TicketingConString.ReDateCancel,
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
