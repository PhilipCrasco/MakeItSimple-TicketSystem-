using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket.GetOpenTicketResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetRequestorTicketConcern;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern
{
    public class GetOpenTicket
    {

        public class GetOpenTicketResult
        {

            public int? RequestedTicketCount { get; set; }
            public int? OpenTicketCount { get; set; }
            public int? CloseTicketCount { get; set; }
            public int? DelayedTicketCount { get; set; }

            public int? TicketConcernId { get; set; }
            public int? RequestConcernId { get; set; }

            public string Ticket_No {  get; set; } 

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
            public bool ? Is_Closed { get; set; }
            public bool ? Is_ReTicket { get; set; }
            public bool ? Is_ReDate {  get; set; }  

        }

        public class GetOpenTicketQuery : UserParams, IRequest<PagedList<GetOpenTicketResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
            public bool? IsClosedApprove { get; set; }
            public bool ? Is_Approve { get; set; }
            public bool ? Is_Transfer { get; set; }
            public bool ? Is_ReTicket { get; set; }

            public bool ? Is_ReDate { get; set; }
            public string UserType { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
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
                var dateToday = DateTime.UtcNow;

                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.Channel)
                    .ThenInclude(x => x.Project)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit);


                if (ticketConcernQuery.Count() > 0)
                {
                    var businessUnitList = await _context.BusinessUnits
                        .FirstOrDefaultAsync(x => x.Id == ticketConcernQuery.First().RequestorByUser.BusinessUnitId);

                    var receiverList = await _context.Receivers
                        .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);

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
                        || x.User.SubUnit.SubUnitName.Contains(request.Search));

                    }

                    if (request.Status != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Status);
                    }

                    if(request.Is_Approve != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsApprove == request.Is_Approve);
                    }

                    if(request.Is_ReDate != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsReDate == request.Is_ReDate);
                    }

                    if (request.Is_Transfer != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsTransfer == request.Is_Transfer);
                    }

                    if (request.Is_ReTicket != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == request.Is_ReTicket);
                    }

                    if (request.IsClosedApprove == true)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsClosedApprove == true);
                    }



                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.IssueHandler)
                        {
                            
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                        }

                        if (request.UserType == TicketingConString.Support)
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


                        if (request.UserType == TicketingConString.Receiver && receiverList != null)
                        {
                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && request.UserId == receiverList.UserId)
                            {

                                var receiver = await _context.TicketConcerns
                                    .Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId)
                                    .ToListAsync();

                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);

                                var requestorSelect = receiver.Select(x => x.Id);

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => receiverContains.Contains(x.RequestorByUser.BusinessUnitId)
                                    && requestorSelect.Contains(x.Id));

                            }
                            else
                            {
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                    }
                    
                }

                var results = ticketConcernQuery.Select(x => new GetOpenTicketResult
                {
                    CloseTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove == true),
                    RequestedTicketCount = ticketConcernQuery.Count(),
                    OpenTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove != true),
                    DelayedTicketCount = ticketConcernQuery.Count(x => x.TargetDate < dateToday && x.IsClosedApprove != true),
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
                                        : "Unknown",

                    Concern_Type = x.TicketType,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Remarks = x.Remarks,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    IsActive = x.IsActive,

                    Is_Closed = x.IsClosedApprove,
                    Is_ReDate = x.IsReDate,
                    Is_ReTicket = x.IsReTicket,
                    Is_Transfer = x.IsTransfer,



                }); ;

                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
