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

        }

        public class RequestTicketNotificationQuery
        {
            public int ? RequestTransactionId { get; set; }

        }

        public class RequestTicketNotificationResultQuery : IRequest<Result>
        {

            public Guid? UserId { get; set; }
            public string Role { get; set; }

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

                var allTicketNotif = new List<RequestConcern>();
                var forTicketNotif = new List<RequestConcern>();
                var currentlyFixingNotif = new List<RequestConcern>();
                var notConfirmNotif = new List<RequestConcern>();
                var doneNotif = new List<RequestConcern>();

                var businessUnitList = new List<BusinessUnit>();                

                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .ToListAsync();

                if(requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles.AsNoTracking().ToListAsync();

                    var requestorPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor)).Select(x => x.UserRoleName).ToList();


                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.IsActive == true)
                        .ToList();


                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.IsReject == false)
                        .ToList();

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


                }

                var notification = requestConcernsQuery
                        .Select(x => new RequestTicketNotificationResult
                        {
                            AllTicketNotif = allTicketNotif.Count(),
                            ForTicketNotif = forTicketNotif.Count(),
                            CurrentlyFixingNotif = currentlyFixingNotif.Count(),
                            NotConfirmNotif = notConfirmNotif.Count(),
                            DoneNotif = doneNotif.Count(),

                           
                        }).DistinctBy(x => x.AllTicketNotif).ToList();

                return Result.Success(notification);

               
            }
        }

    }
}
