using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class ClosingTicketNotification
    {
        public class ClosingTicketNotificationResult
        {
            public int AllClosingNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }
            public int ApproveClosingNotif { get; set; }

        }

        public class ClosingTicketNotificationResultQuery : IRequest<Result>
        {

            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }

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

                var AllClosingNotif = new List<ClosingTicket>();
                var ForApprovalClosingNotif = new List<ClosingTicket>();
                var ApproveClosingNotif = new List<ClosingTicket>();

                var businessUnitList = new List<BusinessUnit>();

                var query = await _context.ClosingTickets
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Where(x => x.IsActive)
                    .ToListAsync();

                if(query.Any())
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .ToListAsync();

                    var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                    var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();


                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = query.Select(x => x.Id);

                        if (request.UserType == TicketingConString.Approver)
                        {

                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {

                                var userApprover = await _context.Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => x.UserId == userApprover.Id)
                                    .Where(x => x.IsApprove == null)
                                    .Select(x => new
                                    {
                                         x.ApproverLevel,
                                         x.IsApprove,
                                         x.ClosingTicketId,
                                         x.UserId,

                                    })
                                    .ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.ClosingTicketId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                query = query
                                    .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                    && userRequestIdApprovalList.Contains(x.Id))
                                    .ToList();

                            }
                        }

                        else if (request.UserType == TicketingConString.Receiver)
                        {

                            var listOfRequest = query.Select(x => new
                            {
                                x.TicketConcern.User.BusinessUnitId

                            }).ToList();

                            foreach (var businessUnit in listOfRequest)
                            {
                                var businessUnitDefault = await _context.BusinessUnits
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == businessUnit.BusinessUnitId && x.IsActive == true);
                                businessUnitList.Add(businessUnitDefault);

                            }

                            var businessSelect = businessUnitList.Select(x => x.Id).ToList();

                            var receiverList = await _context.Receivers
                                .Include(x => x.BusinessUnit)
                                .Where(x => businessSelect.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                                 x.UserId == request.UserId)
                                .ToListAsync();

                            var selectReceiver = receiverList.Select(x => x.BusinessUnitId);

                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                            {

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => filterApproval.Contains(x.ClosingTicketId.Value) && x.IsApprove == null)
                                    .ToListAsync();

                                if (approverTransactList.Any())
                                {
                                    var generatedIdInApprovalList = approverTransactList
                                        .Select(approval => approval.ClosingTicketId);

                                    query = query
                                        .Where(x => !generatedIdInApprovalList.Contains(x.Id))
                                        .ToList();
                                }

                                query = query
                                    .Where(x => selectReceiver.Contains(x.TicketConcern.User.BusinessUnitId))
                                    .ToList();

                            }

                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            query = query.Where(x => x.AddedByUser.Id == request.UserId)
                                .ToList();
                        }
                        else
                        {
                            return Result.Success(query == null);
                        }

                    }

                }

               foreach(var item in query)
                {
                    AllClosingNotif.Add(item);
                }

               var forClosingApproval = query
                    .Where(x => x.IsClosing == false)   
                    .ToList();

                foreach(var item in  forClosingApproval)
                {
                    ForApprovalClosingNotif.Add(item);
                }

               var closingApprove = query
                    .Where(x => x.IsClosing == true)
                    .ToList();

                foreach(var item in closingApprove)
                {
                    ApproveClosingNotif.Add(item);
                }


                var notification = query.Select(x => new ClosingTicketNotificationResult
                {
                    AllClosingNotif = AllClosingNotif.Count(),
                    ForApprovalClosingNotif = forClosingApproval.Count(),
                    ApproveClosingNotif = ApproveClosingNotif.Count(),

                }).DistinctBy(x => x.AllClosingNotif)
                .ToList();

                return Result.Success(notification);

            }
        }
    }
}
