using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class RejectClosingTicket
    {
        public class RejectClosingTicketCommand : IRequest<Result>
        {
            public Guid? RejectClosed_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Reject_Remarks { get; set; }
            public int ? ClosingTicketId { get; set; }

        }

        public class Handler : IRequestHandler<RejectClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var closedTicketExist = await _context.ClosingTickets
                        .FirstOrDefaultAsync(x => x.Id == command.ClosingTicketId);

                if (closedTicketExist == null)
                {
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());
                }

                closedTicketExist.RejectClosedAt = DateTime.Now;
                closedTicketExist.IsRejectClosed = true;
                closedTicketExist.RejectClosedBy = command.RejectClosed_By;
                closedTicketExist.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == closedTicketExist.TicketConcernId);

                ticketConcernExist.IsClosedApprove = null;
                ticketConcernExist.Remarks = command.Reject_Remarks;

                var approverList = await _context.ApproverTicketings
                    .Where(x => x.ClosingTicketId == command.ClosingTicketId)
                    .ToListAsync();

                foreach (var transferTicket in approverList)
                {
                    _context.Remove(transferTicket);
                }

                var ticketHistory = await _context.TicketHistories
                    .Where(x => (x.TicketConcernId == ticketConcernExist.Id
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                     || x.Request.Contains(TicketingConString.NotConfirm))
                    .ToListAsync();

                foreach (var item in ticketHistory)
                {
                    _context.TicketHistories.Remove(item);
                }

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closedTicketExist.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.CloseReject} {userDetails.Fullname}",
                    Remarks = command.Reject_Remarks,
                    
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


        }
    }
}
