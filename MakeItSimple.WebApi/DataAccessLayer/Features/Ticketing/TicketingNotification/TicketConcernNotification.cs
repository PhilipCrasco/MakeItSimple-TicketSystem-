using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
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
            public int AllTicketNotif { get; set; }
            public int ForTicketNotif { get; set;}
            public int CurrentlyFixingNotif { get; set; }
            public int NotConfirmNotif { get; set; }
            public int DoneNotif { get; set; }
            public int ReceiverForApprovalNotif { get; set; }
            public int ReceiverApproveNotif {  get; set; }

        }

        public class RequestTicketNotificationResultQuery : IRequest<List<RequestTicketNotificationResult>>
        {

            public Guid? UserId { get; set; }
            public string Role { get; set; }

        }

        public class Handler : IRequestHandler<RequestTicketNotificationResultQuery, List<RequestTicketNotificationResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<List<RequestTicketNotificationResult>> Handle(RequestTicketNotificationResultQuery request, CancellationToken cancellationToken)
            {

                var allTicketNotif = new List<RequestConcern>();
                var forTicketNotif = new List<RequestConcern>();
                var currentlyFixingNotif = new List<RequestConcern>();
                var notConfirmNotif = new List<RequestConcern>();
                var doneNotif = new List<RequestConcern>();
                var receiverForApprovalNotif = new List<TicketConcern>();
                var receiverApproveNotif = new List<TicketConcern>();

                var businessUnitList = new List<BusinessUnit>();                

                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .Where(x => x.IsActive == true)
                    .ToListAsync();


                if (requestConcernsQuery.Any())
                {
                    var allUserList = await _context.UserRoles.AsNoTracking().ToListAsync();

                    var requestorPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();


                    var ticketStatusList = await _context.TicketConcerns
                        .AsNoTrackingWithIdentityResolution()
                        .Select(x => x.RequestConcernId)
                        .ToListAsync();

                    requestConcernsQuery = requestConcernsQuery
                       .Where(x => ticketStatusList.Contains(x.Id))
                       .ToList();


                    if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.UserId == request.UserId)
                            .ToList();
                    }

                    foreach (var ticket in requestConcernsQuery)
                    {
                        allTicketNotif.Add(ticket);            
                    }

                    var forApprovalTicket = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                        .ToList();

                    foreach (var ticket in forApprovalTicket)
                    {
                        forTicketNotif.Add(ticket);
                    }

                    var currentlyFixing = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .ToList();

                    foreach (var ticket in currentlyFixing)
                    {
                        currentlyFixingNotif.Add(ticket);
                    }

                    var notConfirm  = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .ToList();
                    
                    foreach(var ticket in notConfirm)
                    {
                        notConfirm.Add(ticket);
                    }

                    var done = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .ToList();

                    foreach( var ticket in done)
                    {
                        doneNotif.Add(ticket);
                    }

                    var listOfRequest =  requestConcernsQuery
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
                        .Select(x => x.Id).ToList();

                    var receiverList = await _context.Receivers
                        .AsNoTrackingWithIdentityResolution()
                        .Include(x => x.BusinessUnit)
                        .Where(x => businessSelect.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                         x.UserId == request.UserId)
                        .ToListAsync();

                    var selectReceiver = receiverList.Select(x => x.BusinessUnitId);

                    if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                    {

                        var receiverConcernsQuery = requestConcernsQuery
                            .Where(x => selectReceiver.Contains(x.User.BusinessUnitId))
                            .Select(x => x.Id)
                            .ToList();


                        var forApprovalConcerns = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                            .ToListAsync();

                        foreach(var item in forApprovalConcerns)
                        {
                            receiverForApprovalNotif.Add(item);
                        }

                        var ApproveConcerns = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == true)
                            .ToListAsync();

                        foreach(var item in ApproveConcerns)
                        {
                            receiverApproveNotif.Add(item);
                        }
                    }

                }

                var notification = requestConcernsQuery
                        .Select(x => new RequestTicketNotificationResult
                        {
                            AllTicketNotif = allTicketNotif.Count(),
                            ForTicketNotif = forTicketNotif.Count(),
                            CurrentlyFixingNotif = currentlyFixingNotif.Count(),
                            NotConfirmNotif = notConfirmNotif.Count(),
                            DoneNotif = doneNotif.Count(),
                            ReceiverForApprovalNotif = receiverForApprovalNotif.Count(),
                            ReceiverApproveNotif = receiverApproveNotif.Count(),
                           
                        }).DistinctBy(x => x.AllTicketNotif)
                        .ToList();

                return notification;
               
            }
        }

    }
}
