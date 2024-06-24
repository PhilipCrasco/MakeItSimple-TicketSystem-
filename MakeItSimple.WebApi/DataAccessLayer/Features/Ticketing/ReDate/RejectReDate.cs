using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReDate
{
    public class RejectReDate
    {
        public class RejectReDateCommand : IRequest<Result>
        {
            public Guid? RejectReDate_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Role { get; set; }
            public int TicketReDateId { get; set; }

            public string Reject_Remarks { get; set; }
        }

        public class Handler : IRequestHandler<RejectReDateCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectReDateCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await _context.Users
                  .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var reDateExist = await _context.TicketReDates.
                    FirstOrDefaultAsync(x => x.Id == command.TicketReDateId, cancellationToken);

                

                if (reDateExist == null)
                {
                    return Result.Failure(ReDateError.ReDateIdNotExist());
                }

                var userRoleList = await _context.UserRoles.ToListAsync();

                var approverUserList = await _context.ApproverTicketings
                    .Where(x => x.TicketReDateId == reDateExist.Id)
                    .ToListAsync();

                if (!approverUserList.Any())
                {
                    return Result.Failure(TransferTicketError.NoApproverExist());
                }

                var approverPermission = userRoleList
                .Where(x => x.Permissions.Contains(TicketingConString.Approver))
                .Select(x => x.UserRoleName);

                if (!approverPermission.Any(x => x.Contains(command.Role)))
                {
                    return Result.Failure(TicketRequestError.NotAutorize());
                }

                reDateExist.IsActive = false;
                reDateExist.IsRejectReDate = true;
                reDateExist.RejectReDateBy = command.RejectReDate_By;
                reDateExist.RejectReDateAt = DateTime.Now;
                reDateExist.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == reDateExist.TicketConcernId);

                ticketConcernExist.IsReDate = null;
                ticketConcernExist.Remarks = command.Reject_Remarks;

                foreach (var approverUserId in approverUserList)
                {
                    _context.Remove(approverUserId);
                }

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = reDateExist.TicketConcernId,
                    TransactedBy = reDateExist.ReDateBy,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.ReDate,
                    Status = $"{TicketingConString.ReDateReject} {userDetails.Fullname}"
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
