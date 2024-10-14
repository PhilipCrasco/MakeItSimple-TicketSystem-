using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosing
{
    public partial class ApprovalClosingTicket
    {
        public class Handler : IRequestHandler<ApproveClosingTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.UserRoleName,
                        x.Permissions

                    })
                    .ToListAsync();

                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                foreach (var close in command.ApproveClosingRequests)
                {

                    var closingTicketExist = await _context.ClosingTickets
                        .Include(x => x.TicketConcern)
                        .ThenInclude(x => x.User)
                        .Include(x => x.TicketConcern)
                        .ThenInclude(x => x.RequestorByUser)
                        .FirstOrDefaultAsync(x => x.Id == close.ClosingTicketId);

                    if (closingTicketExist is null)
                        return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                    var closedRequestId = await _context.ApproverTicketings
                        .Where(x => x.ClosingTicketId == closingTicketExist.Id && x.IsApprove == null)
                        .ToListAsync();

                    var ticketHistoryList = await _context.TicketHistories
                        .Where(x => x.TicketConcernId == closingTicketExist.TicketConcernId
                         && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                        .ToListAsync();

                    var selectClosedRequestId = closedRequestId
                        .FirstOrDefault(x => x.ApproverLevel == closedRequestId.Min(x => x.ApproverLevel));

                    if (selectClosedRequestId is not null)
                    {

                        if (closingTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))                    
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        
                        selectClosedRequestId.IsApprove = true;

                        var userApprovalId = await _context.ApproverTicketings
                            .Where(x => x.ClosingTicketId == selectClosedRequestId.ClosingTicketId)
                            .ToListAsync();

                        var validateUserApprover = userApprovalId
                            .FirstOrDefault(x => x.ApproverLevel == selectClosedRequestId.ApproverLevel + 1);


                        await ApprovalTicketHistory(ticketHistoryList,userDetails, command, cancellationToken);

                        if (validateUserApprover is not null)
                        {


                            await ApprovalClosingNotification(closingTicketExist, userDetails, validateUserApprover, command, cancellationToken);

                        }
                        else
                        {
                            await UpdateClosingTicket(closingTicketExist, userDetails,command, cancellationToken);

                        }

                    }
                    else
                    {
                        return Result.Failure(ClosingTicketError.ApproverUnAuthorized());

                    }

                }
                
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }


            private async Task<TicketHistory> ApprovalTicketHistory(List<TicketHistory> ticketHistory, User user, ApproveClosingTicketCommand command,CancellationToken cancellationToken)
            {
                var ticketHistoryApproval = ticketHistory
                    .FirstOrDefault(x => x.Approver_Level != null
                    && x.Approver_Level == ticketHistory.Min(x => x.Approver_Level));

                ticketHistoryApproval.TransactedBy = command.Transacted_By;
                ticketHistoryApproval.TransactionDate = DateTime.Now;
                ticketHistoryApproval.Request = TicketingConString.Approve;
                ticketHistoryApproval.Status = $"{TicketingConString.CloseApprove} {user.Fullname}";
                ticketHistoryApproval.IsApprove = true;

                return ticketHistoryApproval;

            }

            private async Task<ClosingTicket> UpdateClosingTicket(ClosingTicket closingTicket,User user,ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {

                closingTicket.TicketApprover = null;

                closingTicket.TicketApprover = null;
                closingTicket.IsClosing = true;
                closingTicket.ClosingAt = DateTime.Now;
                closingTicket.ClosedBy = command.Closed_By;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == closingTicket.TicketConcernId, cancellationToken);

                ticketConcernExist.IsClosedApprove = true;
                ticketConcernExist.Closed_At = DateTime.Now;
                ticketConcernExist.ClosedApproveBy = command.Closed_By;
                ticketConcernExist.IsDone = true;
                ticketConcernExist.ConcernStatus = TicketingConString.Done;

                var requestConcernExist = await _context.RequestConcerns
                    .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestConcernId);

                requestConcernExist.IsDone = true;
                requestConcernExist.Resolution = closingTicket.Resolution;
                requestConcernExist.ConcernStatus = TicketingConString.NotConfirm;

                await ConfirmationNotification(closingTicket, user, requestConcernExist, command, cancellationToken);

                await ForConfirmationNotification(closingTicket, user, ticketConcernExist, command, cancellationToken);

                return closingTicket;
            }

            private async Task<TicketTransactionNotification> ApprovalClosingNotification(ClosingTicket closingTicket, User user, ApproverTicketing approverTicketing, ApproveClosingTicketCommand command , CancellationToken cancellationToken ) 
            {

                closingTicket.TicketApprover = approverTicketing.UserId;

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {closingTicket.TicketConcernId} is pending for closing approval",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = approverTicketing.UserId.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForClosingTicket,
                    PathId = closingTicket.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);


                return addNewTicketTransactionNotification;
            }

            private async Task<TicketTransactionNotification> ConfirmationNotification(ClosingTicket closingTicket, User user, RequestConcern requestConcern, ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {closingTicket.TicketConcernId} is pending for closing Confirmation",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = requestConcern.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForConfirmation,
                    PathId = closingTicket.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                return addNewTicketTransactionNotification;

            }

            private async Task<TicketTransactionNotification> ForConfirmationNotification(ClosingTicket closingTicket, User user, TicketConcern ticketConcern , ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {closingTicket.TicketConcernId} is waiting for Confirmation",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = ticketConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForConfirmation,
                    PathId = closingTicket.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTransactionConfirmationNotification);

                return addNewTransactionConfirmationNotification;
            }
        }
    }
}
