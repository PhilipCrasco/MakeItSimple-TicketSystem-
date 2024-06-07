using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket;
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
            public Guid? Requestor_By { get; set; }
            public Guid? Approver_By { get; set; }
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

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                foreach (var close in command.ApproveClosingRequests)
                {

                    var closingTicketExist = await _context.ClosingTickets
                        .Include(x =>  x.TicketConcern)
                        .ThenInclude(x => x.User)
                        .FirstOrDefaultAsync(x => x.Id == close.ClosingTicketId);  

                    var closedRequestId = await _context.ApproverTicketings
                        .Where(x => x.ClosingTicketId == closingTicketExist.Id && x.IsApprove == null)
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

                        if (validateUserApprover != null)
                        {
                            closingTicketExist.TicketApprover = validateUserApprover.UserId;
                        }
                        else
                        {
                            closingTicketExist.TicketApprover = null;
                        }

                        var approverLevel = selectClosedRequestId.ApproverLevel == 1 ? $"{selectClosedRequestId.ApproverLevel}st"
                            : selectClosedRequestId.ApproverLevel == 2 ? $"{selectClosedRequestId.ApproverLevel}nd"
                            : selectClosedRequestId.ApproverLevel == 3 ? $"{selectClosedRequestId.ApproverLevel}rd"
                            : $"{selectClosedRequestId.ApproverLevel}th";

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = closingTicketExist.TicketConcernId,
                            ApproverBy = command.Approver_By,
                            RequestorBy = closingTicketExist.AddedBy,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.CloseTicket,
                            Status = $"{TicketingConString.ApproveBy} {approverLevel} approver"
                        };

                        await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

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
                            ticketConcernExist.Remarks = TicketingConString.TicketClosed;
                            ticketConcernExist.ConcernStatus = TicketingConString.Done;

                            var requestConcernExist = await _context.RequestConcerns
                                .FirstOrDefaultAsync(x => x.Id == ticketConcernExist.RequestConcernId);

                            requestConcernExist.IsDone = false;
                            requestConcernExist.ConcernStatus = TicketingConString.Done;


                            var addTicketHistory = new TicketHistory
                            {
                                TicketConcernId = closingTicketExist.TicketConcernId,
                                ApproverBy = command.Approver_By,
                                RequestorBy = closingTicketExist.AddedBy,
                                TransactionDate = DateTime.Now,
                                Request = TicketingConString.CloseTicket,
                                Status = TicketingConString.ReceiverApproveBy
                            };

                            await _context.TicketHistories.AddAsync(addTicketHistory, cancellationToken);

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
