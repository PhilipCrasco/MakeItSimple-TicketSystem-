using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.RequestTicketNotification;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class ClosingTicketNotification
    {
        public class ClosingTicketNotificationResult
        {
            public int ClosingTicketCount { get; set; }
        }

        public class ClosingTicketNotificationQuery
        {
            public int? TicketGeneratorId { get; set; }

        }
        public class ClosingTicketNotificationResultQuery : IRequest<Result>
        {

            public Guid? UserId { get; set; }
            public string Approval { get; set; }
            public string Users { get; set; }
            public string Role { get; set; }
            public bool? IsClosed { get; set; }
            public bool? Reject { get; set; }
            //public Guid? UserApproverId { get; set; }

        }


        public class Handler : IRequestHandler<ClosingTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ClosingTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {
                var query = await _context.ClosingTickets.Include(x => x.User).GroupBy(x => x.TicketGeneratorId).ToListAsync();

                var getQuery = query.Select(x => x.First().User.BusinessUnitId);

                var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => getQuery.Contains(x.Id));
                var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                var userApprover = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                var fillterApproval = query.Select(x => x.First().TicketGeneratorId);


                if (request.Reject != null)
                {
                    query = query.Where(x => x.First().IsRejectClosed == request.Reject).ToList();
                }

                if (request.IsClosed != null)
                {
                    query = query.Where(x => x.First().IsClosing == request.IsClosed).ToList();
                }

                if (TicketingConString.Approval == request.Approval)
                {

                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => x.UserId == userApprover.Id).ToListAsync();
                        var approvalLevelList = approverTransactList.Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null).ToList();
                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.TicketGeneratorId);
                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        query = query.Where(x => userIdsInApprovalList.Contains(x.First().TicketApprover) && userRequestIdApprovalList.Contains(x.First().TicketGeneratorId)).ToList();

                    }

                    else if (request.Role == TicketingConString.Receiver && request.UserId == receiverList.UserId)
                    {
                        var approverTransactList = await _context.ApproverTicketings.Where(x => fillterApproval.Contains(x.TicketGeneratorId) && x.IsApprove == null).ToListAsync();

                        if (approverTransactList != null && approverTransactList.Any())
                        {
                            var generatedIdInApprovalList = approverTransactList.Select(approval => approval.TicketGeneratorId);
                            query = query   .Where(x => !generatedIdInApprovalList.Contains(x.First().TicketGeneratorId)).ToList();
                        }
                        var receiver = await _context.TicketConcerns.Include(x => x.RequestorByUser).Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId).ToListAsync();
                        var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                        query = query.Where(x => receiverContains.Contains(x.First().User.BusinessUnitId)).ToList();

                    }
                    else
                    {
                        query = query.Where(x => x.First().TicketGeneratorId == null).ToList();
                    }

                }

                if (TicketingConString.Users == request.Users)
                {
                    query = query.Where(x => x.First().AddedByUser.Id == request.UserId).ToList();
                }

                var notification = query.Select(x => new ClosingTicketNotificationResult
                {
                    ClosingTicketCount = query.Count()

                }).DistinctBy(x => x.ClosingTicketCount).ToList();


                return Result.Success(notification);

            }
        }
    }
}
