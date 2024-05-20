using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Experimental;
using System.Security.Cryptography.X509Certificates;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class RequestTicketNotification
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
                public string IssueHandler { get; set; }
                public string Approver { get; set; }
                public string Requestor { get; set; }
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
                    .GroupBy(x => x.RequestTransactionId)
                    .ToListAsync();

                var getQuery = query.Select(x => x.First().RequestorByUser.BusinessUnitId);

                var businessUnitList = await _context.BusinessUnits
                    .FirstOrDefaultAsync(x => getQuery.Contains(x.Id));

                var receiverList = await _context.Receivers
                    .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

                var userApprover = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                var fillterApproval = query.Select(x => x.First().RequestTransactionId);


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


                if (request.Approver == TicketingConString.Approver)
                {
                    if (request.Role == TicketingConString.Receiver && request.UserId == receiverList.UserId)
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

                        query = query.Where(x => receiverContains.Contains(x.First().RequestorByUser.BusinessUnitId))
                            .ToList();
                    }

                    else if (request.Role == TicketingConString.Approver)
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


                if (request.Requestor == TicketingConString.Requestor)
                {

                    var requestConcernList = await _context.RequestConcerns.Where(x => x.UserId == request.UserId).ToListAsync();
                    var requestConcernContains = requestConcernList.Select(x => x.UserId);
                    query = query.Where(x => requestConcernContains.Contains(x.First().RequestorBy)).ToList();

                }


                if (request.IssueHandler == TicketingConString.IssueHandler)
                {
                    query = query.Where(x => x.First().UserId == request.UserId).ToList();
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
