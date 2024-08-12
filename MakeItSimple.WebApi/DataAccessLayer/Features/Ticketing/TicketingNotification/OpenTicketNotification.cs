using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
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
            public int AllTicketNotif { get; set; }
            public int PendingTicketNotif { get; set; }
            public int OpenTicketNotif { get; set; }
            public int ForTransferNotif { get; set; }
            public int ForCloseNotif { get; set; }
            public int NotConfirmNotif { get; set; }
            public int ClosedNotif {  get; set; }

        }

        public class OpenTicketNotificationQuery
        {
             public int ? TicketGeneratorId { get; set; }
        }

        public class OpenTicketNotificationResultQuery : IRequest<Result>
        {
            public string UserType { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }

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
                var AllTicketNotif = new List<TicketConcern>();
                var PendingTicketNotif = new List<TicketConcern>();
                var OpenTicketNotif = new List<TicketConcern>();
                var ForTransferNotif = new List<TicketConcern>();
                var ForCloseNotif = new List<TicketConcern>();
                var NotConfirmNotif = new List<TicketConcern>();
                var ClosedNotif = new List<TicketConcern>();

                var query = await _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.RequestConcern)
                    .Include(x => x.User)
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

                        query = query
                        .Where(x => x.IsActive == true)
                        .ToList();

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

                    foreach(var item in query)
                    {
                        AllTicketNotif.Add(item);
                    }

                    var pendingTicket = query
                          .Where(x => x.IsApprove == false)
                          .ToList();

                    foreach(var item in pendingTicket)
                    {
                        PendingTicketNotif.Add(item);
                       
                    }

                   var openTicket = query
                        .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                        && x.IsReTicket != false && x.IsClosedApprove == null)
                        .ToList();

                    foreach (var item in openTicket)
                    {
                       OpenTicketNotif.Add(item);
                    }

                   var forTransferTicket = query
                        .Where(x => x.IsTransfer == false)
                        .ToList();

                    foreach (var item in forTransferTicket)
                    {
                        ForTransferNotif.Add(item);
                    }

                    var forClosedTicket = query
                        .Where(x => x.IsClosedApprove == false)
                        .ToList();

                    foreach(var item in forClosedTicket)
                    {
                        ForCloseNotif.Add(item);
                    }

                    var notConfirmTicket = query
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null)
                        .ToList();

                    foreach(var item in notConfirmTicket)
                    {
                        NotConfirmNotif.Add(item);
                    }

                    var closedTicket = query
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
                        .ToList();

                    foreach (var item in closedTicket)
                    {
                        ClosedNotif.Add(item);
                    }

                }

                var notification = query.Select(x => new OpenTicketNotificationResult
                {
                    AllTicketNotif = AllTicketNotif.Count(),
                    PendingTicketNotif = PendingTicketNotif.Count(),
                    OpenTicketNotif = OpenTicketNotif.Count(),
                    ForTransferNotif = ForTransferNotif.Count(),
                    ForCloseNotif = ForCloseNotif.Count(),
                    NotConfirmNotif = NotConfirmNotif.Count(),
                    ClosedNotif = ClosedNotif.Count(),

                }).DistinctBy(x => x.AllTicketNotif)
                .ToList();

                return Result.Success(notification);
            }
        }
    }
}
