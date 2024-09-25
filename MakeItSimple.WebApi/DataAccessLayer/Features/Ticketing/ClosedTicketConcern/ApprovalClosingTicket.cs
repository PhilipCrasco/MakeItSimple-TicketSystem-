using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class ApprovalClosingTicket
    {
        public class ApproveClosingTicketCommand : IRequest<Result>
        {
            public Guid? Closed_By { get; set; }public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Modules { get; set; }
            public List<ApproveClosingRequest> ApproveClosingRequests { get; set; }
           public class ApproveClosingRequest
           {
                public int ? ClosingTicketId { get; set; }
           }

        }
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
                    .ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                foreach (var close in command.ApproveClosingRequests)
                {

                    var closingTicketExist = await _context.ClosingTickets
                        .Include(x =>  x.TicketConcern)
                        .ThenInclude(x => x.User)
                        .Include(x => x.TicketConcern)
                        .ThenInclude(x => x.RequestorByUser)
                        .FirstOrDefaultAsync(x => x.Id == close.ClosingTicketId);  

                    var closedRequestId = await _context.ApproverTicketings
                        .Where(x => x.ClosingTicketId == closingTicketExist.Id && x.IsApprove == null)
                        .ToListAsync();

                    var ticketHistoryList = await _context.TicketHistories
                        .Where(x => x.TicketConcernId == closingTicketExist.TicketConcernId 
                         && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                        .ToListAsync();

                    var selectClosedRequestId = closedRequestId
                        .FirstOrDefault(x => x.ApproverLevel == closedRequestId.Min(x => x.ApproverLevel));

                    if (selectClosedRequestId != null)
                    {

                        if (closingTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))
                        {
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                        }

                        selectClosedRequestId.IsApprove = true;

                        var userApprovalId = await _context.ApproverTicketings
                            .Where(x => x.ClosingTicketId == selectClosedRequestId.ClosingTicketId)
                            .ToListAsync();

                        var validateUserApprover = userApprovalId
                            .FirstOrDefault(x => x.ApproverLevel == selectClosedRequestId.ApproverLevel + 1);

                        var ticketHistoryApproval = ticketHistoryList
                            .FirstOrDefault(x => x.Approver_Level != null
                            && x.Approver_Level == ticketHistoryList.Min(x => x.Approver_Level));

                        ticketHistoryApproval.TransactedBy = command.Transacted_By;
                        ticketHistoryApproval.TransactionDate = DateTime.Now;
                        ticketHistoryApproval.Request = TicketingConString.Approve;
                        ticketHistoryApproval.Status = $"{TicketingConString.CloseApprove} {userDetails.Fullname}";
                        ticketHistoryApproval.IsApprove = true;

                        if (validateUserApprover != null)
                        {
                            closingTicketExist.TicketApprover = validateUserApprover.UserId;

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is pending for closing approval",
                                AddedBy = userDetails.Id,
                                Created_At = DateTime.Now,
                                ReceiveBy = validateUserApprover.UserId.Value,
                                Modules = command.Modules,

                            };

                            await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);
                        }
                        else
                        {
                            closingTicketExist.TicketApprover = null;

                            var requestorReceiver = await _context.Receivers
                                .FirstOrDefaultAsync(x => x.BusinessUnitId == closingTicketExist.TicketConcern.RequestorByUser.BusinessUnitId);

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is pending for closing approval",
                                AddedBy = userDetails.Id,
                                Created_At = DateTime.Now,
                                ReceiveBy = requestorReceiver.UserId.Value,
                                Modules = PathConString.Approval,
                                Modules_Parameter = PathConString.ForClosingTicket,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);
                        }

                    }
                    else
                    {
                        var businessUnitList = await _context.BusinessUnits
                            .FirstOrDefaultAsync(x => x.Id == closingTicketExist.TicketConcern.User.BusinessUnitId);

                        var receiverList = await _context.Receivers
                            .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

                        if (receiverList.UserId == command.Users && receiverPermissionList.Any(x => x.Contains(command.Role)))
                        {

                            closingTicketExist.IsClosing = true;
                            closingTicketExist.ClosingAt = DateTime.Now;
                            closingTicketExist.ClosedBy = command.Closed_By; 

                            var ticketConcernExist = await _context.TicketConcerns
                                .FirstOrDefaultAsync(x => x.Id == closingTicketExist.TicketConcernId, cancellationToken);

                            ticketConcernExist.IsClosedApprove = true;
                            ticketConcernExist.Closed_At = DateTime.Now;
                            ticketConcernExist.ClosedApproveBy = command.Closed_By;
                            ticketConcernExist.IsDone = true;
                            ticketConcernExist.ConcernStatus = TicketingConString.Done;

                            var requestConcernExist = await _context.RequestConcerns
                                .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestConcernId);

                            requestConcernExist.IsDone = true;
                            requestConcernExist.Resolution  = closingTicketExist.Resolution;
                            requestConcernExist.ConcernStatus = TicketingConString.NotConfirm;

                            var ticketHistory = ticketHistoryList
                                .FirstOrDefault(x => x.Id == ticketHistoryList.Max(x => x.Id));

                            ticketHistory.TransactedBy = command.Transacted_By;
                            ticketHistory.TransactionDate = DateTime.Now;
                            ticketHistory.Request = TicketingConString.TicketClosed;
                            ticketHistory.Status = TicketingConString.CloseApproveReceiver;
                            ticketHistory.IsApprove = true;


                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is pending for closing Confirmation",
                                AddedBy = userDetails.Id,
                                Created_At = DateTime.Now,
                                ReceiveBy = requestConcernExist.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);



                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is waiting for Confirmation",
                                AddedBy = userDetails.Id,
                                Created_At = DateTime.Now,
                                ReceiveBy = ticketConcernExist.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await _context.TicketTransactionNotifications.AddAsync(addNewTransactionConfirmationNotification);

                        }
                        else
                        {
                            return Result.Failure(ClosingTicketError.ApproverUnAuthorized());
                        }

                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
               
            }
        }
    }
}
