using Azure.Core;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketConcernNotification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            public int AllTransferNotif { get; set; }
            public int ForApprovalTransferNotif { get; set; }
            public int ApproveTransferNotif { get; set; }

            public int AllClosingNotif { get; set; }
            public int ForApprovalClosingNotif { get; set; }
            public int ApproveClosingNotif { get; set; }


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

                var allRequestTicketNotif = new List<RequestConcern>();
                var forTicketNotif = new List<RequestConcern>();
                var currentlyFixingNotif = new List<RequestConcern>();
                var notConfirmNotif = new List<RequestConcern>();
                var doneNotif = new List<RequestConcern>();
                var receiverForApprovalNotif = new List<TicketConcern>();
                var receiverApproveNotif = new List<TicketConcern>();

                var allTicketNotif = new List<TicketConcern>();
                var pendingTicketNotif = new List<TicketConcern>();
                var openTicketNotif = new List<TicketConcern>();
                var forTransferNotif = new List<TicketConcern>();
                var forCloseNotif = new List<TicketConcern>();
                var notCloseConfirmCloseNotif = new List<TicketConcern>();
                var closedNotif = new List<TicketConcern>();


                var allTransferNotif = new List<TransferTicketConcern>();
                var forApprovalTransferNotif = new List<TransferTicketConcern>();
                var approveTransferNotif = new List<TransferTicketConcern>();

                var allClosingNotif = new List<ClosingTicket>();
                var forApprovalClosingNotif = new List<ClosingTicket>();
                var approveClosingNotif = new List<ClosingTicket>();

                var businessUnitRequestList = new List<BusinessUnit>();
                var businessUnitCloseList = new List<BusinessUnit>();


                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Include(x => x.TicketConcerns)
                    .Include(x => x.User)
                    .Where(x => x.IsActive == true)
                    .ToListAsync();

                var ticketConcernQuery = await _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.RequestConcern)
                    .Include(x => x.User)
                    .Include(x => x.RequestorByUser)
                    .ToListAsync();


                var transferQuery = await _context.TransferTicketConcerns
                .AsNoTracking()
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.User)
                .Where(x => x.IsActive == true)
                .ToListAsync();

                var closeQuery = await _context.ClosingTickets
                    .AsNoTracking()
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Where(x => x.IsActive)
                    .ToListAsync();


                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .ToListAsync();

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

                var ticketStatusList = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Select(x => x.RequestConcernId)
                    .ToListAsync();


                if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                {

                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => ticketStatusList.Contains(x.Id))
                        .ToList();

                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.UserId == request.UserId && requestConcernsQuery.Any())
                        .ToList();

                    foreach (var ticket in requestConcernsQuery)
                    {
                        allRequestTicketNotif.Add(ticket);
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

                    var notConfirm = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .ToList();

                    foreach (var ticket in notConfirm)
                    {
                        notConfirm.Add(ticket);
                    }

                    var done = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .ToList();

                    foreach (var ticket in done)
                    {
                        doneNotif.Add(ticket);
                    }


                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.IsApprove == true && x.UserId == request.UserId && ticketConcernQuery.Any())
                        .ToList();

                    foreach (var item in ticketConcernQuery)
                    {
                        allTicketNotif.Add(item);
                    }

                    var pendingTicket = ticketConcernQuery
                          .Where(x => x.IsApprove == false)
                          .ToList();

                    foreach (var item in pendingTicket)
                    {
                        pendingTicketNotif.Add(item);

                    }

                    var openTicket = ticketConcernQuery
                         .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                         && x.IsReTicket != false && x.IsClosedApprove == null)
                         .ToList();
                    foreach (var item in openTicket)
                    {
                        openTicketNotif.Add(item);
                    }

                    var forTransferTicket = ticketConcernQuery
                         .Where(x => x.IsTransfer == false)
                         .ToList();

                    foreach (var item in forTransferTicket)
                    {
                        forTransferNotif.Add(item);
                    }

                    var forClosedTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == false)
                        .ToList();

                    foreach (var item in forClosedTicket)
                    {
                        forCloseNotif.Add(item);
                    }

                    var notConfirmTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null)
                        .ToList();

                    foreach (var item in notConfirmTicket)
                    {
                        notCloseConfirmCloseNotif.Add(item);
                    }

                    var closedTicket = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true)
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
                            x.ClosingTicketId,
                            x.UserId,

                        }).ToListAsync();


                    if(transferQuery.Any())
                    {
                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.TransferTicketConcernId);

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId);

                        transferQuery = transferQuery
                            .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id))
                            .ToList();

                        foreach (var item in transferQuery)
                        {
                            allTransferNotif.Add(item);
                        }

                        var forApprovalTransfer = transferQuery
                               .Where(x => x.IsTransfer == false)
                               .ToList();

                        foreach (var item in forApprovalTransfer)
                        {
                            forApprovalTransferNotif.Add(item);
                        }

                        var approveTransfer = transferQuery
                               .Where(x => x.IsTransfer == true)
                               .ToList();

                        foreach (var item in approveTransfer)
                        {
                            approveTransferNotif.Add(item);
                        }

                    }

                    if(closeQuery.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList.Select(x => x.ClosingTicketId);
                        var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                        closeQuery = closeQuery
                            .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                            && userRequestIdApprovalList.Contains(x.Id))
                            .ToList();
                    }

                }

                if (receiverPermissionList.Any(x => x.Contains(request.Role)) && requestConcernsQuery.Any())
                {
                    if(requestConcernsQuery.Any())
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

                            businessUnitRequestList.Add(businessUnitDefault);
                        }

                        var businessSelect = businessUnitRequestList
                            .Select(x => x.Id)
                            .ToList();

                        var receiverList = await _context.Receivers
                            .AsNoTrackingWithIdentityResolution()
                            .Include(x => x.BusinessUnit)
                            .Where(x => businessSelect
                            .Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                             x.UserId == request.UserId)
                            .ToListAsync();

                        if (receiverList.Any())
                        {
                            var selectReceiver = receiverList
                                .Select(x => x.BusinessUnitId)
                                .ToList();

                            var receiverConcernsQuery = requestConcernsQuery
                                    .Where(x => selectReceiver.Contains(x.User.BusinessUnitId))
                                    .Select(x => x.Id)
                                    .ToList();

                            var forApprovalConcerns = await _context.TicketConcerns
                                .AsNoTrackingWithIdentityResolution()
                                .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                                .ToListAsync();

                            foreach (var item in forApprovalConcerns)
                            {
                                receiverForApprovalNotif.Add(item);
                            }

                            var ApproveConcerns = await _context.TicketConcerns
                                .AsNoTrackingWithIdentityResolution()
                                .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == true)
                                .ToListAsync();

                            foreach (var item in ApproveConcerns)
                            {
                                receiverApproveNotif.Add(item);
                            }
                        }

                    }

                    if (closeQuery.Any())
                    {
                        var filterApproval = closeQuery.Select(x => x.Id);

                        var listOfRequest = closeQuery.Select(x => new
                        {
                            x.TicketConcern.User.BusinessUnitId

                        }).ToList();

                        foreach (var businessUnit in listOfRequest)
                        {
                            var businessUnitDefault = await _context.BusinessUnits
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == businessUnit.BusinessUnitId && x.IsActive == true);
                            businessUnitCloseList.Add(businessUnitDefault);

                        }

                        var businessSelect = businessUnitCloseList.Select(x => x.Id).ToList();

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

                                closeQuery = closeQuery
                                    .Where(x => !generatedIdInApprovalList.Contains(x.Id))
                                    .ToList();
                            }

                            closeQuery = closeQuery
                                .Where(x => selectReceiver.Contains(x.TicketConcern.User.BusinessUnitId))
                                .ToList();
                        }

                    }

                }

                if (closeQuery.Any())
                {
                    foreach (var item in closeQuery)
                    {
                        allClosingNotif.Add(item);
                    }

                    var forClosingApproval = closeQuery
                         .Where(x => x.IsClosing == false)
                         .ToList();

                    foreach (var item in forClosingApproval)
                    {
                        forApprovalClosingNotif.Add(item);
                    }

                    var closingApprove = closeQuery
                         .Where(x => x.IsClosing == true)
                         .ToList();

                    foreach (var item in closingApprove)
                    {
                        approveClosingNotif.Add(item);
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

                    AllTransferNotif = allTransferNotif.Count(),
                    ForApprovalTransferNotif = approveTransferNotif.Count(),
                    ApproveTransferNotif = approveTransferNotif.Count(),

                    AllClosingNotif = allClosingNotif.Count(),
                    ForApprovalClosingNotif = forApprovalClosingNotif.Count(),
                    ApproveClosingNotif = approveClosingNotif.Count(),


                };


                return Result.Success(notification);

            }
        }
    }
}
