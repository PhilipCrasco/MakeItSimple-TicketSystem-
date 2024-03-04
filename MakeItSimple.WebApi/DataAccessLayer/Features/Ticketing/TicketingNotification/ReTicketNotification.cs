using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TransferTicketNotification;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class ReTicketNotification
    {

        public class ReTicketNotificationResult
        {
            public int ReTicketCount { get; set; }
        }

        public class ReTicketNotificationQuery
        {
            public int? RequestGeneratorId { get; set; }
        }

        public class ReTicketNotificationResultQuery : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public string Approval { get; set; }
            public string Users { get; set; }
            public string Role { get; set; }
            public bool? IsReTicket { get; set; }
            public bool? IsReject { get; set; }
        }

        public class Handler : IRequestHandler<ReTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ReTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {
                var query = await _context.ReTicketConcerns.GroupBy(x => x.RequestGeneratorId).ToListAsync();

                var fillterApproval = query.Select(x => x.First().RequestGeneratorId);

                if (TicketingConString.Approval == request.Approval)
                {
                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == request.UserId).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.RequestGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        query = query.Where(x => userIdsInApprovalList.Contains(x.First().TicketApprover) && userRequestIdApprovalList.Contains(x.First().RequestGeneratorId)).ToList();
                    }
                    else if (TicketingConString.Admin == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.RequestGeneratorId) && x.IsApprove == null).ToListAsync();
                        if (approverTransactList != null && approverTransactList.Any())
                        {
                            var generatedIdInApprovalList = approverTransactList.Select(approval => approval.RequestGeneratorId);
                            query = query.Where(x => !generatedIdInApprovalList.Contains(x.First().RequestGeneratorId)).ToList();
                        }
                    }
                    else
                    {
                        query = query.Where(x => x.First().RequestGeneratorId == null).ToList();
                    }

                }


                if (TicketingConString.Users == request.Users)
                {
                    query = query.Where(x => x.First().UserId == request.UserId).ToList();
                }

                if (request.IsReTicket != null)
                {
                    query = query.Where(x => x.First().IsReTicket == request.IsReTicket).ToList();
                }

                if (request.IsReject != null)
                {
                    query = query.Where(x => x.First().IsRejectReTicket == request.IsReject).ToList();
                }

                var notification = query.Select(x => new ReTicketNotificationResult
                {
                    ReTicketCount = query.Count(),

                }).DistinctBy(x => x.ReTicketCount).ToList();

                return Result.Success(notification);
            }
        }
    }
}
