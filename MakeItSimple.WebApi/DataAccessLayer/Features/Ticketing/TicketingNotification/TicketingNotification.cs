using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany.GetCompanyResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class TicketingNotification
    {
        public class TicketingNotifResult
        {
            public int AllRequestTicketNotif { get; set; }
            public int ForTicketNotif { get; set; }
            public int CurrentlyFixingNotif { get; set; }
            public int NotConfirmNotif { get; set; }
            public int DoneNotif { get; set; }
            public int ReceiverForApprovalNotif { get; set; }
            public int ReceiverApproveNotif { get; set; }
            public int AllTicketNotif { get; set; }
            public int PendingTicketNotif { get; set; }
            public int OpenTicketNotif { get; set; }
            public int ForTransferNotif { get; set; }
            public int ForCloseNotif { get; set; }
            public int NotConfirmCloseNotif { get; set; }
            public int ClosedNotif { get; set; }
            public int ForApprovalTransferNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }

        }

        public class BusinessUnitNotif
        {
            public int Id { get; set; }
            public bool Is_Active {  get; set; }
        }

        public class TicketingNotificationCommand : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public string Role {  get; set; }

        }


        public class Handler : IRequestHandler<TicketingNotificationCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IMediator _mediator;

            public Handler(MisDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;   
            }

            public async Task<Result> Handle(TicketingNotificationCommand request, CancellationToken cancellationToken)
            {

                var allRequestTicketNotif = new List<int>();
                var forTicketNotif = new List<int>();
                var currentlyFixingNotif = new List<int>();
                var notConfirmNotif = new List<int>();
                var doneNotif = new List<int>();
                var receiverForApprovalNotif = new List<int?>();
                var receiverApproveNotif = new List<int?>();

                var allTicketNotif = new List<int>();
                var pendingTicketNotif = new List<int>();
                var openTicketNotif = new List<int>();
                var forTransferNotif = new List<int>();
                var forCloseNotif = new List<int>();
                var notCloseConfirmCloseNotif = new List<int>();
                var closedNotif = new List<int>();

                var forApprovalTransferNotif = new List<int>();
                var forApprovalClosingNotif = new List<int>();

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id, 
                        x.UserRoleName,
                        x.Permissions

                    }).ToListAsync();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();
 
                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .Where(x => x.IsActive == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.TicketConcerns,
                        x.User,
                        x.UserId,
                        x.ConcernStatus,
                        x.Is_Confirm,                 
                        x.IsActive,
                        x.IsDone,

                    }).ToListAsync();

                var ticketConcernQuery = await _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.RequestConcern)
                    .Include(x => x.User)
                    .Include(x => x.RequestorByUser)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestConcern,
                        x.RequestConcernId,
                        x.User,
                        x.UserId,
                        x.RequestorByUser,
                        x.IsActive,
                        x.IsDone,
                        x.IsApprove,
                        x.IsClosedApprove,
                        x.IsTransfer,
                        x.IsReject,
                      
                    }).ToListAsync();


                var transferQuery = await _context.TransferTicketConcerns
                .AsNoTracking()
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.User)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsRejectTransfer == false)
                .Select(x => new
                {
                    x.Id,
                    x.TicketConcern,
                    x.TicketConcernId,
                    x.TicketApprover,
                    x.IsTransfer
            


                }).ToListAsync();


                var closeQuery = await _context.ClosingTickets
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Where(x => x.IsActive)
                    .Where(x => x.IsRejectClosed == false)
                    .Select(x => new
                    {
                        x.Id,
                        x.TicketConcern,
                        x.IsClosing,
                        x.TicketApprover

                    }).ToListAsync();


                if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                {

                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.UserId == request.UserId && requestConcernsQuery.Any())
                        .ToList();

                    var allRequestTicket = requestConcernsQuery
                        .Select(x => x.Id)
                        .ToList();

                    foreach (var ticket in allRequestTicket)
                    {
                        allRequestTicketNotif.Add(ticket);
                    }

                    var forApprovalTicket = requestConcernsQuery
                            .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                            .Select(x => x.Id)
                            .ToList();

                    foreach (var ticket in forApprovalTicket)
                    {
                        forTicketNotif.Add(ticket);
                    }

                    var currentlyFixing = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .Select(x => x.Id)
                                    .ToList();

                    foreach (var ticket in currentlyFixing)
                    {
                        currentlyFixingNotif.Add(ticket);
                    }

                    var notConfirm = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .Select (x => x.Id)
                        .ToList();

                    foreach (var ticket in notConfirm)
                    {
                        notConfirmNotif.Add(ticket);
                    }

                    var done = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .Select (x => x.Id)
                        .ToList();

                    foreach (var ticket in done)
                    {
                        doneNotif.Add(ticket);
                    }


                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.IsApprove == true && x.UserId == request.UserId && ticketConcernQuery.Any())
                        .ToList();

                    var allTicketConcern = ticketConcernQuery
                        .Select(x => x.Id)
                        .ToList();

                    foreach (var item in allTicketConcern)
                    {
                        allTicketNotif.Add(item);
                    }

                    var pendingTicket = ticketConcernQuery
                          .Where(x => x.IsApprove == false)
                          .Select(x => x.Id)
                          .ToList();

                    foreach (var item in pendingTicket)
                    {
                        pendingTicketNotif.Add(item);

                    }

                    var openTicket = ticketConcernQuery
                         .Where(x => x.IsApprove == true && x.IsTransfer != false
                         &&  x.IsClosedApprove == null)
                         .Select(x => x.Id)
                         .ToList();

                    foreach (var item in openTicket)
                    {
                        openTicketNotif.Add(item);
                    }

                    var forTransferTicket = ticketConcernQuery
                         .Where(x => x.IsTransfer == false)
                         .Select (x => x.Id)
                         .ToList();

                    foreach (var item in forTransferTicket)
                    {
                        forTransferNotif.Add(item);
                    }

                    var forClosedTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == false)
                        .Select(x => x.Id)  
                        .ToList();

                    foreach (var item in forClosedTicket)
                    {
                        forCloseNotif.Add(item);
                    }

                    var notConfirmTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null)
                        .Select(x => x.Id)  
                        .ToList();

                    foreach (var item in notConfirmTicket)
                    {
                        notCloseConfirmCloseNotif.Add(item);
                    }

                    var closedTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
                        .Select(x => x.Id)
                        .ToList();

                    foreach (var item in closedTicket)
                    {
                        closedNotif.Add(item);
                    }    
                }

                if (approverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    var userApprover = await _context.Users
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.Username,
                           
                        }).FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                    var approverTransactList = await _context.ApproverTicketings
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x => x.UserId == userApprover.Id)
                        .Where(x => x.IsApprove == null)
                        .Select(x => new
                        {
                            x.ApproverLevel,
                            x.IsApprove,
                            x.TransferTicketConcernId,
                            x.ClosingTicketId,
                            x.UserId,

                        }).ToListAsync();

                    if(transferQuery.Any())
                    {
                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.TransferTicketConcernId);

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId);

                        var forApprovalTransfer = transferQuery
                               .Where(x => x.IsTransfer == false)
                               .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id))
                               .Select(x => x.Id)
                               .ToList();

                        foreach (var item in forApprovalTransfer)
                        {
                            forApprovalTransferNotif.Add(item);
                        }

                    }

                    if(closeQuery.Any())
                    {
                         
                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.ClosingTicketId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                      var closeForApproval = closeQuery
                            .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id))
                            .Select(x => x.Id)
                            .ToList();

                        foreach(var ticket in closeForApproval)
                        {
                            forApprovalClosingNotif.Add(ticket);
                        }
                    }

                }


                if (receiverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    if (requestConcernsQuery.Any())
                    {
                        var listOfRequest = requestConcernsQuery
                            .Select(x => x.User.BusinessUnitId)
                            .ToList();

                        var receiverList = await _context.Receivers
                            .AsNoTrackingWithIdentityResolution()
                            .Include(x => x.BusinessUnit)
                            .Where(x => x.IsActive == true)
                            .Where(x => listOfRequest
                            .Contains(x.BusinessUnitId))                       
                            .Select(x => x.BusinessUnitId)
                            .ToListAsync();

                        var receiverConcernsQuery = requestConcernsQuery
                                .Where(x => receiverList.Contains(x.User.BusinessUnitId))
                                .Select(x => x.Id)
                                .ToList();

                        var forApprovalConcerns = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        receiverForApprovalNotif = forApprovalConcerns;

                        var ApproveConcerns = await _context.TicketConcerns
                            .AsNoTrackingWithIdentityResolution()
                            .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value))
                            .Where(x => x.IsApprove == true)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        receiverApproveNotif = ApproveConcerns;
                    }


                    if (closeQuery.Any())
                    {
                        var filterApproval = closeQuery
                            .Select(x => x.Id)
                            .ToList();

                        var listOfRequest = closeQuery
                            .Select(x => x.TicketConcern.User.BusinessUnitId)
                            .ToList();

                        var receiverList = await _context.Receivers
                            .Include(x => x.BusinessUnit)
                            .Where(x => listOfRequest.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                             x.UserId == request.UserId)
                            .Select (x => x.BusinessUnitId)
                            .ToListAsync();


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

                                closeQuery = closeQuery
                                    .Where(x => !generatedIdInApprovalList.Contains(x.Id))
                                    .ToList();
                            }

                           var forClosingApproval = closeQuery
                                .Where(x => receiverList.Contains(x.TicketConcern.User.BusinessUnitId))
                                .Where(x => x.IsClosing == false)
                                .Select(x => x.Id)
                                .ToList();
 
                            forApprovalClosingNotif = forClosingApproval;


                        }

                    }

                }


                var notification = new TicketingNotifResult
                {
                    AllRequestTicketNotif = allRequestTicketNotif.Count(),
                    ForTicketNotif = forTicketNotif.Count(),
                    CurrentlyFixingNotif = currentlyFixingNotif.Count(),
                    NotConfirmNotif = notConfirmNotif.Count(),
                    DoneNotif = doneNotif.Count(),
                    ReceiverForApprovalNotif = receiverForApprovalNotif.Count(),
                    ReceiverApproveNotif = receiverApproveNotif.Count(),

                    AllTicketNotif = allTicketNotif.Count(),
                    PendingTicketNotif = pendingTicketNotif.Count(),
                    OpenTicketNotif = openTicketNotif.Count(),
                    ForTransferNotif = forTransferNotif.Count(),
                    ForCloseNotif = forCloseNotif.Count(),
                    NotConfirmCloseNotif = notCloseConfirmCloseNotif.Count(),
                    ClosedNotif = closedNotif.Count(),

                    ForApprovalTransferNotif = forApprovalTransferNotif.Count(),
                    ForApprovalClosingNotif = forApprovalClosingNotif.Count(),

                };


                return Result.Success(notification);

            }
        }
    }
}
