using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketConcernNotification;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class OpenTicketNotification
    {
        public class OpenTicketNotificationResult
        {
            public int OpenTicketCount { get; set; }
            
        }

        public class OpenTicketNotificationQuery
        {
             public int ? TicketGeneratorId { get; set; }
        }

        public class OpenTicketNotificationResultQuery : IRequest<Result>
        {
            public bool? Status { get; set; }
            public string Concern_Status { get; set; }

            public string History_Status { get; set; }
            public string UserType { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }


        public class Handler : IRequestHandler<OpenTicketNotificationResultQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(OpenTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {

                var businessUnitList = new List<BusinessUnit>();

                var query = await _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.RequestorByUser)
                    .ToListAsync();

                if(query.Any())
                {

                    var filterApproval = query.Select(x => x.Id);

                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .ToListAsync();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                    var issueHandlerPermissionList = allUserList
                        .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
                        .Select(x => x.UserRoleName).
                        ToList();

                    var supportPermissionList = allUserList
                        .Where(x => x.Permissions.Contains(TicketingConString.Support))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (request.Status is not null)
                    {
                        query = query.Where(x => x.IsActive == request.Status)
                            .ToList();
                    }

                    if (!string.IsNullOrEmpty(request.Concern_Status))
                    {
                        switch (request.Concern_Status)
                        {
                            case TicketingConString.PendingRequest:
                                query = query
                                    .Where(x => x.IsApprove == false)
                                    .ToList();
                                break;

                            case TicketingConString.Open:
                                query = query
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                                    && x.IsReTicket != false && x.IsClosedApprove == null)
                                    .ToList();
                                break;

                            case TicketingConString.ForTransfer:

                                query = query
                                    .Where(x => x.IsTransfer == false)
                                    .ToList();
                                break;


                            case TicketingConString.ForClosing:
                                query = query
                                    .Where(x => x.IsClosedApprove == false)
                                    .ToList();
                                break;

                            case TicketingConString.NotConfirm:
                                query = query
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null)
                                    .ToList();
                                break;

                            case TicketingConString.Closed:
                                query = query
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
                                    .ToList();
                                break;

                            default:
                                return Result.Success(query == null);

                        }


                    }

                    if (!string.IsNullOrEmpty(request.History_Status))
                    {
                        switch (request.History_Status)
                        {
                            case TicketingConString.PendingRequest:
                                query = query
                                    .Where(x => x.IsApprove == false)
                                    .ToList();
                                break;

                            case TicketingConString.Open:
                                query = query
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                                    && x.IsReTicket != false && x.IsClosedApprove == null)
                                    .ToList();
                                break;

                            case TicketingConString.ForTransfer:

                                query = query
                                    .Where(x => x.IsTransfer == false)
                                    .ToList();
                                break;


                            case TicketingConString.ForClosing:
                                query = query
                                    .Where(x => x.IsClosedApprove == false)
                                    .ToList();
                                break;

                            case TicketingConString.NotConfirm:
                                query = query
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null)
                                    .ToList();
                                break;

                            case TicketingConString.Closed:
                                query = query
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
                                    .ToList();
                                break;

                            default:
                                return Result.Success(query == null);

                        }

                    }

                    if (request.Date_From is not null && request.Date_From is not null)
                    {
                        query = query
                            .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value)
                            .ToList();
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        if (request.UserType == TicketingConString.IssueHandler)
                        {
                            query = query
                                .Where(x => x.UserId == request.UserId)
                                .ToList();
                        }
                        else if (request.UserType == TicketingConString.Support)
                        {

                            if (supportPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                var channelUserValidation = await _context.ChannelUsers
                                    .AsNoTracking()
                                    .Where(x => x.UserId == request.UserId)
                                    .ToListAsync();

                                var channelSelectValidation = channelUserValidation.Select(x => x.ChannelId);

                                query = query
                                    .Where(x => channelSelectValidation.Contains(x.ChannelId.Value))
                                    .ToList();
                            }
                            else
                            {
                                return Result.Success(query == null);
                            }

                        }
                        else if (request.UserType == TicketingConString.Receiver)
                        {
                            var listOfRequest = query.Select(x => new
                            {
                                x.User.BusinessUnitId

                            });

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

                                query = query
                                    .Where(x => selectReceiver.Contains(x.RequestorByUser.BusinessUnitId))
                                    .ToList();
                            }
                            else
                            {
                                return Result.Success(query == null);
                            }

                        }
                        else
                        {
                            return Result.Success(query == null);
                        }
                    }

                }

                var notification = query.Select(x => new OpenTicketNotificationResult
                {
                    OpenTicketCount = query.Count()

                }).DistinctBy(x => x.OpenTicketCount)
                .ToList();

                return Result.Success(notification);
            }
        }
    }
}
