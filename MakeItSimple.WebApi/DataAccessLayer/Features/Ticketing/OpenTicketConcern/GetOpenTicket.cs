﻿using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern
{
    public class GetOpenTicket
    {
        public class GetOpenTicketResult
        {

            public int? RequestedTicketCount { get; set; }
            public int? OpenTicketCount { get; set; }
            public int? ForConfirmationCount { get; set; }
            public int? CloseTicketCount { get; set; }
            public int? DelayedTicketCount { get; set; }
            public int ? ForClosingTicketCount { get; set; }
            public int? TicketConcernId { get; set; }
            public int? RequestConcernId { get; set; }

            public string Closed_Status { get; set; }

            public string Concern_Description { get; set; }

            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }

            public int? UnitId { get; set; }
            public string Unit_Name { get; set; }
            public int? SubUnitId { get; set; }
            public string SubUnit_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }

            public Guid? UserId { get; set; }
            public string Issue_Handler { get; set; }

            public int? CategoryId { get; set; }
            public string Category_Description { get; set; }
            public int? SubCategoryId { get; set; }
            public string SubCategory_Description { get; set; }

            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }

            public string Ticket_Status { get; set; }
            public string Concern_Type { get; set; }

            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool IsActive { get; set; }
            public string Remarks { get; set; }
            public bool? Done { get; set; }

            public bool ? Is_Transfer { get; set; }
            public DateTime ? Transfer_At {  get; set; }
            public bool ? Is_Closed { get; set; }
            public DateTime ? Closed_At { get; set; }

            public List<GetForClosingTicket> GetForClosingTickets { get; set; }
            public List<GetForTransferTicket> GetForTransferTickets { get; set; }

            public class GetForClosingTicket
            {
                public int ? ClosingTicketId { get; set; }
                public string Resolution { get; set; }
                public string Remarks { get; set; }
                public bool ? IsApprove { get; set; }
                     
                public List<GetAttachmentForClosingTicket> GetAttachmentForClosingTickets { get; set; }
                public class GetAttachmentForClosingTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            }

            public class GetForTransferTicket
            {
                public int? TransferTicketConcernId { get; set; }
                public string Transfer_Remarks { get; set; }
                public bool? IsApprove { get; set; }

                public List<GetAttachmentForTransferTicket> GetAttachmentForTransferTickets { get; set; }
                public class GetAttachmentForTransferTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            } 

        }

        public class GetOpenTicketQuery : UserParams, IRequest<PagedList<GetOpenTicketResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
            public string Concern_Status { get; set; }

            public string History_Status { get; set; }
            public string UserType { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

        public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                int hoursDiff = 24;
                var businessUnitList = new List<BusinessUnit>();

                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.Channel)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketConcerns);
                    

                if (ticketConcernQuery.Any())
                {

                    var fillterApproval = ticketConcernQuery.Select(x => x.Id);

                    var allUserList = await _context.UserRoles
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

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.User.Fullname.Contains(request.Search)
                        || x.User.SubUnit.SubUnitName.Contains(request.Search)
                        || x.TicketNo.Contains(request.Search));

                    }

                    if (request.Status != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Status);
                    }

                    if(!string.IsNullOrEmpty(request.Concern_Status))
                    {
                        switch(request.Concern_Status)
                        {
                            case TicketingConString.PendingRequest :
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                                    && x.IsReTicket != false && x.IsClosedApprove == null);
                                break;

                            case TicketingConString.ForTransfer:

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsTransfer == false);
                                break;


                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (!string.IsNullOrEmpty(request.History_Status))
                    {
                        switch (request.History_Status)
                        {
                            case TicketingConString.PendingRequest:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false && x.IsReDate != false
                                    && x.IsReTicket != false && x.IsClosedApprove == null);
                                break;

                            case TicketingConString.ForTransfer:

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsTransfer == false);
                                break;
                                
                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (request.Date_From is not null && request.Date_From is not null)
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        if (request.UserType == TicketingConString.IssueHandler)
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                        }
                        else if (request.UserType == TicketingConString.Support)
                        {
                            if (supportPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                var channelUserValidation = await _context.ChannelUsers
                                    .Where(x => x.UserId == request.UserId)
                                    .ToListAsync();

                                var channelSelectValidation = channelUserValidation.Select(x => x.ChannelId);

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => channelSelectValidation.Contains(x.ChannelId.Value));
                            }
                            else
                            {
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                        else if (request.UserType == TicketingConString.Receiver)
                        {
                            var listOfRequest = await ticketConcernQuery.Select(x => new
                            {
                                x.User.BusinessUnitId

                            }).ToListAsync();

                            foreach (var businessUnit in listOfRequest)
                            {
                                var businessUnitDefault = await _context.BusinessUnits
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

                                //var receiver = await _context.TicketConcerns
                                //    .Include(x => x.RequestorByUser)
                                //    .Where(x => selectReceiver.Contains(x.RequestorByUser.BusinessUnitId))
                                //    .ToListAsync();

                                //var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                //var requestorSelect = receiver.Select(x => x.Id);

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => selectReceiver.Contains(x.RequestorByUser.BusinessUnitId));
                            }
                            else
                            {
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                        else
                        {
                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                    
                }

                var confirmConcernList = ticketConcernQuery
                    .Where(x => x.RequestConcern.Is_Confirm == null && x.RequestConcern.IsDone == true)
                    .ToList();

                var results = ticketConcernQuery.Select(x => new GetOpenTicketResult
                {
                    CloseTicketCount = ticketConcernQuery.Count(x => x.RequestConcern.Is_Confirm == true && x.IsClosedApprove == true),
                    RequestedTicketCount = ticketConcernQuery.Count(),
                    OpenTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove != true),
                    ForConfirmationCount = ticketConcernQuery.Count(x => x.RequestConcern.Is_Confirm == null && x.IsClosedApprove == true),
                    DelayedTicketCount = ticketConcernQuery.Count(x => x.TargetDate < dateToday && x.IsClosedApprove != true),
                    ForClosingTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove == false),
                    Closed_Status = x.TargetDate.Value.Day >= x.Closed_At.Value.Day ? TicketingConString.OnTime : TicketingConString.Delay,
                    TicketConcernId = x.Id,
                    RequestConcernId = x.RequestConcernId,
                    Concern_Description = x.ConcernDetails,
                    Requestor_By = x.RequestorBy,
                    Requestor_Name = x.RequestorByUser.Fullname,

                    DepartmentId = x.RequestorByUser.DepartmentId,
                    Department_Name = x.RequestorByUser.Department.DepartmentName,

                    UnitId = x.User.UnitId,
                    Unit_Name = x.User.Units.UnitName,
                    SubUnitId = x.User.SubUnitId,
                    SubUnit_Name = x.User.SubUnit.SubUnitName,
                    ChannelId = x.ChannelId,
                    Channel_Name = x.Channel.ChannelName,

                    UserId = x.UserId,
                    Issue_Handler = x.User.Fullname,

                    CategoryId = x.CategoryId,
                    Category_Description = x.Category.CategoryDescription,

                    SubCategoryId = x.SubCategoryId,
                    SubCategory_Description = x.SubCategory.SubCategoryDescription,

                    Start_Date = x.StartDate,
                    Target_Date = x.TargetDate,

                    Ticket_Status = x.IsApprove == false ? TicketingConString.PendingRequest
                                        : x.IsApprove == true && x.IsReTicket != false && x.IsTransfer != false && x.IsReDate != false && x.IsClosedApprove == null ? TicketingConString.OpenTicket
                                        : x.IsTransfer == false ? TicketingConString.ForTransfer
                                        : x.IsReTicket == false ? TicketingConString.ForReticket
                                        : x.IsReDate == false ? TicketingConString.ForReDate
                                        : x.IsClosedApprove == false ? TicketingConString.ForClosing
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null ? TicketingConString.NotConfirm
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true ? TicketingConString.Closed
                                        : "Unknown",

                    Concern_Type = x.TicketType,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Remarks = x.Remarks,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    IsActive = x.IsActive,

                    Is_Closed = x.IsClosedApprove,
                    Closed_At = x.Closed_At,
                    Is_Transfer = x.IsTransfer,
                    Transfer_At = x.TransferAt,

                    GetForClosingTickets = x.ClosingTickets
                    .Where(x => x.IsActive == true && x.IsRejectClosed == false)
                    .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
                    .Select(x => new GetOpenTicketResult.GetForClosingTicket
                    {
                        ClosingTicketId = x.Id,
                        Remarks = x.RejectRemarks,
                        Resolution = x.Resolution,
                        IsApprove = x.ApproverTickets.Any(x => x.IsApprove == null) ? false : true,

                        GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForClosingTicket.GetAttachmentForClosingTicket
                        {
                            TicketAttachmentId = x.Id,
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileSize = x.FileSize,

                        }).ToList(),


                    }).ToList(),

                    GetForTransferTickets = x.TransferTicketConcerns
                    .Where(x => x.IsActive == true && x.IsTransfer == false)
                    .Select(x => new GetOpenTicketResult.GetForTransferTicket
                    {
                        TransferTicketConcernId = x.Id,
                        Transfer_Remarks = x.TransferRemarks,
                        IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                        GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
                        {
                            TicketAttachmentId = x.Id,
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileSize = x.FileSize,

                        }).ToList(),

                    }).ToList(),

                });

                foreach (var confirmConcern in confirmConcernList)
                {

                    var daysClose = confirmConcern.Closed_At.Value.Day - dateToday.Day;

                    daysClose = Math.Abs(daysClose) * (1);

                    if (daysClose >= 1)
                    {
                        daysClose = daysClose * 24;
                    }

                    var hourConvert = (daysClose + confirmConcern.Closed_At.Value.Hour) - dateToday.Hour;

                    if (hourConvert >= hoursDiff)
                    {
                        var requestConcern = await _context.RequestConcerns
                            .FirstOrDefaultAsync(x => x.Id == confirmConcern.RequestConcernId);

                        requestConcern.Is_Confirm = true;
                        requestConcern.Confirm_At = DateTime.Today;
                        requestConcern.ConcernStatus = TicketingConString.Done;

                        var ticketConcernExist = await _context.TicketConcerns
                            .FirstOrDefaultAsync(x => x.Id == confirmConcern.Id);

                        var ticketHistory = await _context.TicketHistories
                            .Where(x => x.TicketConcernId == ticketConcernExist.Id
                             && x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                            .FirstOrDefaultAsync();

                        ticketHistory.TicketConcernId = ticketConcernExist.Id;
                        ticketHistory.TransactedBy = request.UserId;
                        ticketHistory.TransactionDate = DateTime.Now;
                        ticketHistory.Request = TicketingConString.CloseTicket;
                        ticketHistory.Status = TicketingConString.CloseConfirm;
              
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                }

                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
