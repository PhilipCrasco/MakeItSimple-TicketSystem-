using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class TicketConcernNotification
    {

        public class RequestTicketNotificationResult
        {
            public int RequestTicketCount { get; set; }
        }

        public class RequestTicketNotificationQuery
        {
            public int ? RequestTransactionId { get; set; }

        }


        public class RequestTicketNotificationResultQuery : IRequest<Result>
        {

                public Guid? UserId { get; set; }
                public string Role { get; set; }
                //public string IssueHandler { get; set; }
                public string Approver { get; set; }

                public string UserType { get; set; }
                //public string Requestor { get; set; }
                public bool ? Status { get; set; }
                public bool? Reject { get; set; }
                public bool? Approval { get; set; }

        }

        public class Handler : IRequestHandler<RequestTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RequestTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {

                var query = await _context.TicketConcerns
                    .Include(x => x.RequestorByUser)
                    .GroupBy(x => new
                    {
                        x.RequestTransactionId,
                        x.UserId
                    }).ToListAsync();


                var getQuery = query.Select(x => x.First().RequestorByUser.BusinessUnitId);

                var businessUnitList = await _context.BusinessUnits
                    .FirstOrDefaultAsync(x => getQuery.Contains(x.Id));

                var receiverList = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

                var userApprover = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                var fillterApproval = query.Select(x => x.First().RequestTransactionId);

                var allUserList = await _context.UserRoles.ToListAsync();

                var receiverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                var requestorPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                var approverPermissionList = allUserList.Where(x => x.Permissions
                .Contains(TicketingConString.Approver)).Select(x => x.UserRoleName).ToList();


                if (request.Status != null)
                {
                    query = query.Where(x => x.First().IsActive == request.Status).ToList();
                }

                if (request.Approval != null)
                {
                    query = query.Where(x => x.First().IsApprove == request.Approval).ToList();
                }

                if (request.Reject != null)
                {
                    query = query.Where(x => x.First().IsReject == request.Reject).ToList();
                }


                if (!string.IsNullOrEmpty(request.UserType))
                {

                    if (request.UserType == TicketingConString.Approver)
                    {
                        if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList != null)
                        { 
                            if (request.UserId == receiverList.UserId)
                            {

                                var approverTransactList = await _context.ApproverTicketings
                                    .Where(x => fillterApproval.Contains(x.RequestTransactionId) && x.IsApprove == null)
                                    .ToListAsync();

                                if (approverTransactList != null && approverTransactList.Any())
                                {
                                    var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestTransactionId);
                                    query = query.Where(x => !generatedIdInApprovalList.Contains(x.First().RequestTransactionId)).ToList();
                                }

                                var receiver = await _context.TicketConcerns
                                    .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId)
                                    .ToListAsync();

                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                var requestorSelect = receiver.Select(x => x.RequestTransactionId);

                                query = query.Where(x => receiverContains.Contains(x.First().RequestorByUser.BusinessUnitId)
                                && requestorSelect.Contains(x.First().RequestTransactionId))
                                    .ToList();

                            }
                            else
                            {
                                query = query.Where(x => x.First().RequestTransactionId == null).ToList();
                            }


                        }

                        else if (approverPermissionList.Any(x => x.Contains(request.Role)))
                        {
                            var approverTransactList = await _context.ApproverTicketings
                                .Where(x => x.UserId == userApprover.Id)
                                .ToListAsync();

                            var approvalLevelList = approverTransactList
                                .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null)
                                .ToList();

                            var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestTransactionId);
                            var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);

                            query = query.Where(x => userIdsInApprovalList.Contains(x.First().TicketApprover)
                            && userRequestIdApprovalList.Contains(x.First().RequestTransactionId))
                                .ToList();

                        }
                        else
                        {
                            query = query.Where(x => x.First().RequestTransactionId == null).ToList();
                        }

                    }


                    if (request.UserType == TicketingConString.Requestor)
                    {
                        if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                        {
                            var requestConcernList = await _context.RequestConcerns.Where(x => x.UserId == request.UserId).ToListAsync();
                            var requestConcernContains = requestConcernList.Select(x => x.UserId);
                            query = query.Where(x => requestConcernContains.Contains(x.First().RequestorBy)).ToList();
                        }
                        else
                        {
                            query = query.Where(x => x.First().RequestTransactionId == null).ToList();
                        }

                    }


                    if (request.UserType == TicketingConString.IssueHandler)
                    {
                        if (issueHandlerPermissionList.Any(x => x.Contains(request.Role)))
                        {
                            query = query.Where(x => x.First().UserId == request.UserId).ToList();
                        }
                        else
                        {
                            query = query.Where(x => x.First().RequestTransactionId == null).ToList();
                        }
                    }

                }

                var notification = query.Select(x => new RequestTicketNotificationResult
                {
                    RequestTicketCount = query.Count()

                }).DistinctBy(x => x.RequestTicketCount).ToList();

                return Result.Success(notification);
            }
        }

    }
}
