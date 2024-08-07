using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestorTicketConcern;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            public string UserType { get; set; }
            public string Concern_Status { get; set; }
            public bool? Status { get; set; }
            public bool? Is_Reject { get; set; }
            public bool? Is_Approve { get; set; }

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

                var businessUnitList = new List<BusinessUnit>();

                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .ToListAsync();

                if(requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles.AsNoTracking().ToListAsync();

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
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsActive == request.Status)
                            .ToList();
                    }

                    if (request.Is_Approve != null)
                    {
                        var ticketStatusList = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => x.IsApprove == request.Is_Approve)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => ticketStatusList.Contains(x.Id))
                           .ToList();
                    }


                    if (request.Is_Reject != null)
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsReject == request.Is_Reject)
                            .ToList();
                    }


                    if (request.Concern_Status is not null)
                    {

                        switch (request.Concern_Status)
                        {

                            case TicketingConString.Approval:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                                    .ToList();
                                break;
                            case TicketingConString.OnGoing:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .ToList();
                                break;

                            case TicketingConString.NotConfirm:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                                    .ToList();
                                break;

                            case TicketingConString.Done:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                                    .ToList();
                                break;
                            default:
                                return Result.Success(requestConcernsQuery == null);

                        }
                    }


                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        if (request.UserType == TicketingConString.Requestor)
                        {
                            if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.UserId == request.UserId)
                                    .ToList();
                            }
                            else
                            {
                                return Result.Success(requestConcernsQuery == null);
                            }
                        }

                        if (request.UserType == TicketingConString.Receiver && requestConcernsQuery.Any())
                        {
                            var listOfRequest = requestConcernsQuery
                                .Select(x => new
                                {
                                    x.User.BusinessUnitId

                                }).ToList();


                            foreach (var businessUnit in listOfRequest)
                            {
                                var businessUnitDefault = await _context.BusinessUnits
                                    .AsNoTrackingWithIdentityResolution()
                                    .FirstOrDefaultAsync(x => x.Id == businessUnit.BusinessUnitId && x.IsActive == true);
                                businessUnitList.Add(businessUnitDefault);

                            }

                            var businessSelect = businessUnitList
                                .Select(x => x.Id)
                                .ToList();

                            var receiverList = await _context.Receivers
                                .AsNoTrackingWithIdentityResolution()
                                .Include(x => x.BusinessUnit)
                                .Where(x => businessSelect.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                                 x.UserId == request.UserId)
                                .ToListAsync();

                            var selectReceiver = receiverList.Select(x => x.BusinessUnitId);

                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                            {

                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => selectReceiver.Contains(x.User.BusinessUnitId))
                                    .ToList();

                            }
                            else
                            {
                                return Result.Success(requestConcernsQuery == null);
                            }

                        }
                    }
                     
                }

                var notification = requestConcernsQuery
                        .Select(x => new RequestTicketNotificationResult
                        {
                            RequestTicketCount = requestConcernsQuery.Count()

                        }).DistinctBy(x => x.RequestTicketCount).ToList();

                return Result.Success(notification);


            }
        }

    }
}
