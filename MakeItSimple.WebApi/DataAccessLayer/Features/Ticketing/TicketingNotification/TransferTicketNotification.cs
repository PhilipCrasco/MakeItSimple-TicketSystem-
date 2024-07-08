using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.EntityFrameworkCore;

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
            public string Role {  get; set; }
            public string Approval {  get; set; }
            public string Users { get; set; }
            public bool? IsReject { get; set; }
            public bool? IsTransfer { get; set; }
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
                    .GroupBy(x => x.Id
                ).ToListAsync();

                var fillterApproval = query.Select(x => x.First().Id);


                if (TicketingConString.Users == request.Users)
                {
                    query = query.Where(x => x.First().AddedBy == request.UserId).ToList();
                }

                if (request.IsTransfer != null)
                {
                    query = query.Where(x => x.First().IsTransfer == request.IsTransfer).ToList();
                }

                if (request.IsReject != null)
                {
                    query = query.Where(x => x.First().IsRejectTransfer == request.IsReject).ToList();
                }

                if (TicketingConString.Approval == request.Approval)
                {
                    if (request.UserId != null && TicketingConString.Approver == request.Role)
                    {
                        var approverTransactList = await _context.ApproverTicketings
                            .Where(x => x.UserId == request.UserId)
                            .ToListAsync();

                        var approvalLevelList = approverTransactList
                            .Where(x => x.ApproverLevel == approverTransactList.First().ApproverLevel && x.IsApprove == null)
                            .ToList();

                        var userRequestIdApprovalList = approvalLevelList.Select(x => x.Id);

                        var userIdsInApprovalList = approvalLevelList.Select(approval => approval.UserId);
                        query = query.Where(x => userIdsInApprovalList.Contains(x.First().TicketApprover) 
                        && userRequestIdApprovalList.Contains(x.First().Id))
                            .ToList();
                    }
                    else
                    {
                        query = query.Where(x => x.First().Id == null).ToList();    
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
