using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer
{
    public partial class ApprovedTransferTicket
    {

        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var allUserList = await _context.UserRoles.ToListAsync();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                var transferTicketExist = await _context.TransferTicketConcerns
                    .Include(x => x.TicketConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());

                var transferApprover = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .FirstOrDefaultAsync();

                if (transferApprover is not null)
                {

                    if (!approverPermissionList.Any(x => x.Contains(command.Role)))
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    
                    if (transferTicketExist.TicketApprover != command.Users)
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    
                    transferApprover.IsApprove = true;

                    var ticketHistoryApproval = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == transferTicketExist.TicketConcernId
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                    .FirstOrDefaultAsync(x => x.Approver_Level != null);

                    ticketHistoryApproval.TransactedBy = command.Transacted_By;
                    ticketHistoryApproval.TransactionDate = DateTime.Now;
                    ticketHistoryApproval.Request = TicketingConString.Approve;
                    ticketHistoryApproval.Status = $"{TicketingConString.TransferApprove} {userDetails.Fullname}";
                    ticketHistoryApproval.IsApprove = true;

                        transferTicketExist.TicketApprover = null;

                        transferTicketExist.IsTransfer = true;
                        transferTicketExist.TransferBy = transferTicketExist.TicketConcern.UserId;
                        transferTicketExist.TransferAt = DateTime.Now;

                        var ticketConcernExist = await _context.TicketConcerns
                            .Include(x => x.RequestorByUser)
                            .FirstOrDefaultAsync(x => x.Id == transferTicketExist.TicketConcernId);

                        ticketConcernExist.IsTransfer = null;
                        ticketConcernExist.Remarks = transferTicketExist.TransferRemarks;
                        ticketConcernExist.UserId = transferTicketExist.TransferTo;
                        ticketConcernExist.TargetDate = command.Target_Date;

                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"New request concern number {ticketConcernExist.RequestConcernId} has received",
                            AddedBy = userDetails.Id,
                            Created_At = DateTime.Now,
                            ReceiveBy = transferTicketExist.TransferTo.Value,
                            Modules = PathConString.ReceiverConcerns,
                            PathId = ticketConcernExist.RequestConcernId.Value,

                        };

                        await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                        var addNewTransferTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"Ticket concern number {ticketConcernExist.RequestConcernId} has transfer",
                            AddedBy = userDetails.Id,
                            Created_At = DateTime.Now,
                            ReceiveBy = transferTicketExist.TransferBy.Value,
                            Modules = PathConString.ConcernTickets,
                            Modules_Parameter = PathConString.ForTransfer,
                            PathId = ticketConcernExist.RequestConcernId.Value,

                        };

                        await _context.TicketTransactionNotifications.AddAsync(addNewTransferTransactionNotification);

                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
