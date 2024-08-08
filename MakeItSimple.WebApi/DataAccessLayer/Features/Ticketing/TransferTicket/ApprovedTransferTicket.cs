using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public class ApprovedTransferTicket
    {

        public class ApprovedTransferTicketCommand : IRequest<Result>
        {
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public int TransferTicketId { get; set; }

        }

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
                {
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());
                }

                var transferRequestTicketId = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .ToListAsync();

                var ticketHistoryList = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == transferTicketExist.TicketConcernId
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                    .ToListAsync();

                var selectTransferRequestId = transferRequestTicketId
                    .FirstOrDefault(x => x.ApproverLevel == transferRequestTicketId.Min(x => x.ApproverLevel));

                if (selectTransferRequestId != null)
                {

                    if (!approverPermissionList.Any(x => x.Contains(command.Role)))
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }

                    if (transferTicketExist.TicketApprover != command.Users)
                    {
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                    }

                    selectTransferRequestId.IsApprove = true;

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TransferTicketConcernId == selectTransferRequestId.TransferTicketConcernId)
                        .ToListAsync();

                    var validateUserApprover = userApprovalId
                        .FirstOrDefault(x => x.ApproverLevel == selectTransferRequestId.ApproverLevel + 1);

                    var ticketHistoryApproval = ticketHistoryList
                        .FirstOrDefault(x => x.Approver_Level != null
                        && x.Approver_Level == ticketHistoryList.Min(x => x.Approver_Level));

                    ticketHistoryApproval.TransactedBy = command.Transacted_By;
                    ticketHistoryApproval.TransactionDate = DateTime.Now;
                    ticketHistoryApproval.Request = TicketingConString.Approve;
                    ticketHistoryApproval.Status = $"{TicketingConString.TransferApprove} {userDetails.Fullname}";
                    ticketHistoryApproval.IsApprove = true;

                    if (validateUserApprover != null)
                    {
                        transferTicketExist.TicketApprover = validateUserApprover.UserId;

                    }
                    else
                    {
                        transferTicketExist.TicketApprover = null;

                        transferTicketExist.IsTransfer = true;
                        transferTicketExist.TransferBy = transferTicketExist.TicketConcern.UserId;
                        transferTicketExist.TransferAt = DateTime.Now;

                        var ticketConcernExist = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.Id == transferTicketExist.TicketConcernId);

                        ticketConcernExist.IsTransfer = true;
                        ticketConcernExist.TransferAt = DateTime.Now;
                        ticketConcernExist.TransferBy = transferTicketExist.TicketConcern.UserId;

                        ticketConcernExist.IsApprove = false;
                        ticketConcernExist.IsTransfer = true;
                        ticketConcernExist.Remarks = transferTicketExist.TransferRemarks;
                        ticketConcernExist.ChannelId = null;
                        ticketConcernExist.UserId = null;
                        ticketConcernExist.CategoryId = null;
                        ticketConcernExist.SubCategoryId = null;
                        ticketConcernExist.StartDate = null;
                        ticketConcernExist.TargetDate = null;
                        ticketConcernExist.IsAssigned = null;

                       
                    }

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
