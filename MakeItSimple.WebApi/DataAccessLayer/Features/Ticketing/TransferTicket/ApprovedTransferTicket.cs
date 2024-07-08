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
            public Guid? Transfer_By { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            //public Guid? Approver_By { get; set; }
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
                    .Where(x => x.TicketConcernId == transferTicketExist.TicketConcernId)
                    .ToListAsync();

                var ticketHistory = ticketHistoryList
                    .FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

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

                    if (ticketHistory.Request == TicketingConString.Approval)
                    {

                        ticketHistory.TransactedBy = command.Transacted_By;
                        ticketHistory.TransactionDate = DateTime.Now;
                        ticketHistory.Request = TicketingConString.Approve;
                        ticketHistory.Status = $"{TicketingConString.TransferApprove} {userDetails.Fullname}";
                    }

                    var approverLevel = selectTransferRequestId.ApproverLevel == 1 ? $"{selectTransferRequestId.ApproverLevel}st"
                        : selectTransferRequestId.ApproverLevel == 2 ? $"{selectTransferRequestId.ApproverLevel}nd"
                        : selectTransferRequestId.ApproverLevel == 3 ? $"{selectTransferRequestId.ApproverLevel}rd"
                        : $"{selectTransferRequestId.ApproverLevel}th";

                    if (validateUserApprover != null)
                    {
                        transferTicketExist.TicketApprover = validateUserApprover.UserId;

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = transferTicketExist.TicketConcernId,
                            TransactedBy = command.Transacted_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Transfer,
                            Status = $"{TicketingConString.TransferForApproval} {approverLevel} Approver"
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

                    }
                    else
                    {
                        transferTicketExist.TicketApprover = null;

                        transferTicketExist.IsTransfer = true;
                        transferTicketExist.TransferBy = command.Transfer_By;
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
