using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransferTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class TransferTicketNotification 
    {

        public class TransferTicketNotificationResult
        {
            public int TransferTicketCount { get; set; }
        }

        public class TransferTicketNotificationQuery
        {
            public int ? TicketGeneratorId { get; set; }   
        }

        public class TransferTicketNotificationResultQuery : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool? IsTransfer { get; set; }
            public bool? IsReject { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<TransferTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(TransferTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {

                var query = await _context.TransferTicketConcerns
                                .Include(x => x.TicketConcern)
                                .ThenInclude(x => x.User)
                                .ToListAsync();

                if(query.Any()) 
                {
                    var allUserList = await _context.UserRoles.AsNoTracking().ToListAsync();

                    var approverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();

                    var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                    if (request.Status != null)
                    {
                        query = query
                            .Where(x => x.IsActive == request.Status)
                            .ToList();
                    }

                    if (request.IsTransfer != null)
                    {

                        query = query
                            .Where(x => x.IsTransfer == request.IsTransfer)
                            .ToList();
                    }

                    if (request.IsReject != null)
                    {
                        query = query
                            .Where(x => x.IsRejectTransfer == request.IsReject)
                            .ToList();
                    }

                    if (request.UserType == TicketingConString.Approver)
                    {

                        if (approverPermissionList.Any(x => x.Contains(request.Role)))
                        {

                            var userApprover = await _context.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);


                            var approverTransactList = await _context.ApproverTicketings
                                .AsNoTrackingWithIdentityResolution()
                                .Where(x => x.UserId == userApprover.Id)
                                .Where(x => x.IsApprove == null)
                                .Select(x => new
                                {
                                    ApproverLevel = x.ApproverLevel,
                                    IsApprove = x.IsApprove,
                                    TransferTicketConcernId = x.ClosingTicketId,
                                    UserId = x.UserId,

                                })
                                .ToListAsync();

                            var userRequestIdApprovalList = approverTransactList.Select(x => x.TransferTicketConcernId);
                            var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                            query = query
                                .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                && userRequestIdApprovalList.Contains(x.Id))
                                .ToList();
                        }
                        else
                        {
                            return Result.Success(query == null);
                        }

                    }

                    if (request.UserType == TicketingConString.IssueHandler)
                    {
                        query = query
                            .Where(x => x.AddedByUser.Id == request.UserId)
                            .ToList();
                    }


                }

                var notification = query.Select(x => new TransferTicketNotificationResult
                {
                    TransferTicketCount = query.Count(),    

                }).DistinctBy(x => x.TransferTicketCount).ToList();


                return Result.Success(notification);
            }
        }
    }
}
