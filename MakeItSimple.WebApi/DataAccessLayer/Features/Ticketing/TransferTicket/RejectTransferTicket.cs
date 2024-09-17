using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class RejectTransferTicket
    {

        public class RejectTransferTicketCommand : IRequest<Result>
        { 
            public Guid ? RejectTransfer_By { get; set; }
            public Guid ? Transacted_By { get; set; }
            public string Role { get; set; }
            public int TransferTicketId { get; set; }

            public string Modules { get; set; }

            public string Reject_Remarks { get; set; }
        }

        public class Handler : IRequestHandler<RejectTransferTicketCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RejectTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var transferTicketExist = await _context.TransferTicketConcerns.
                    FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist == null)
                {
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                }

                var userRoleList = await _context.UserRoles.ToListAsync();

                var approverUserList = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id)
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

                transferTicketExist.IsActive = false;
                transferTicketExist.IsRejectTransfer = true;
                transferTicketExist.RejectTransferBy = command.RejectTransfer_By;
                transferTicketExist.RejectTransferAt = DateTime.Now;
                transferTicketExist.RejectRemarks = command.Reject_Remarks;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == transferTicketExist.TicketConcernId);

                ticketConcernExist.IsTransfer = null;
                ticketConcernExist.Remarks = command.Reject_Remarks;

                foreach (var approverUserId in approverUserList)
                {
                    _context.Remove(approverUserId);
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
                    TicketConcernId = transferTicketExist.TicketConcernId,
                    TransactedBy = transferTicketExist.TransferBy,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.TransferReject} {userDetails.Fullname}",
                    Remarks = command.Reject_Remarks
                };

                await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);


                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Transfer request for ticket number {transferTicketExist.TicketConcernId} was rejected.",
                    AddedBy = userDetails.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketConcernExist.UserId.Value,
                    Modules = command.Modules,

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
