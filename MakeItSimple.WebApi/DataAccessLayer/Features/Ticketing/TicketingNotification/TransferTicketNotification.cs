using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
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

            public int AllTransferNotif { get; set; }
            public int ForTransferClosingNotif { get; set; }
            public int ApproveTransferNotif { get; set; }
        }

        public class TransferTicketNotificationResultQuery : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }


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

                var AllTransferNotif = new List<TransferTicketConcern>();
                var ForApprovalTransferNotif = new List<TransferTicketConcern>();
                var ApproveTransferNotif = new List<TransferTicketConcern>();

                var query = await _context.TransferTicketConcerns
                                .AsNoTracking()
                                .Include(x => x.TicketConcern)
                                .ThenInclude(x => x.User)
                                .Where(x => x.IsActive == true)
                                .ToListAsync();

                if(query.Any()) 
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .ToListAsync();

                    var approverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Approver))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    var issueHandlerPermissionList = allUserList
                        .Where(x => x.Permissions
                    .Contains(TicketingConString.IssueHandler))
                        .Select(x => x.UserRoleName)
                        .ToList();


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
                                     x.ApproverLevel,
                                     x.IsApprove,
                                     x.TransferTicketConcernId,
                                     x.UserId,

                                }).ToListAsync();

                            var userRequestIdApprovalList = approverTransactList
                                .Select(x => x.TransferTicketConcernId);

                            var userIdsInApprovalList = approverTransactList
                                .Select(approval => approval.UserId);

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

                    foreach (var item in query)
                    {
                        AllTransferNotif.Add(item);
                    }

                    var forApprovalTransfer = query
                           .Where(x => x.IsTransfer == false)
                           .ToList();

                    foreach (var item in forApprovalTransfer)
                    {
                        ForApprovalTransferNotif.Add(item);
                        
                    }

                    var approveTransfer = query
                           .Where(x => x.IsTransfer == true)
                           .ToList();

                    foreach (var item in approveTransfer)
                    {
                        ApproveTransferNotif.Add(item);
                    }

                }

                var notification = query.Select(x => new TransferTicketNotificationResult
                {
                    AllTransferNotif = AllTransferNotif.Count(),    
                    ForTransferClosingNotif  = ApproveTransferNotif.Count(),
                    ApproveTransferNotif = ApproveTransferNotif.Count(),
                   

                }).DistinctBy(x => x.AllTransferNotif)
                    .ToList();


                return Result.Success(notification);
            }
        }
    }
}
