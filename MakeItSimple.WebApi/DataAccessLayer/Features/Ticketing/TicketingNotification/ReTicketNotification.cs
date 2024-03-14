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
            public int? TicketGeneratorId { get; set; }
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
                var query = await _context.ReTicketConcerns.GroupBy(x => x.TicketGeneratorId).ToListAsync();

                var fillterApproval = query.Select(x => x.First().TicketGeneratorId);

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


                if (TicketingConString.Approval == request.Approval)
                {
                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == request.UserId).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.TicketGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        query = query.Where(x => userIdsInApprovalList.Contains(x.First().TicketApprover) && userRequestIdApprovalList.Contains(x.First().TicketGeneratorId)).ToList();
                    }
                    else
                    {
                        query = query.Where(x => x.First().TicketGeneratorId == null).ToList();
                    }

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
