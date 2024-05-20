using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket.GetOpenTicketResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.GetOpenTicket.GetOpenTicketResult.OpenTicket;


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
            public int? RequestTransactionId { get; set; }
            public string Description { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }

            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }

            public Guid? UserId { get; set; }
            public string Issue_Handler { get; set; }

            public int? UnitId { get; set; }
            public string Unit_Name { get; set; }

            public int? SubUnitId { get; set; }
            public string SubUnit_Name { get; set; }
            public int? ProjectId { get; set; }
            public string Project_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }

            public string Concern_Type { get; set; }

            public List<OpenTicket> OpenTickets { get; set; }
            public class OpenTicket
            {

                public int? TicketConcernId { get; set; }
                public int? RequestConcernId { get; set; }
                public string Concern_Description { get; set; }
                public string Category_Description { get; set; }
                public string SubCategory_Description { get; set; }
                public string Ticket_Status { get; set; }
                public DateTime? Start_Date { get; set; }
                public DateTime? Target_Date { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
                public bool IsActive { get; set; }
                public string Remarks { get; set; }
                public bool? Done { get; set; }

            }

        }

        public class GetOpenTicketQuery : UserParams, IRequest<PagedList<GetOpenTicketResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
            public bool? IsClosedApprove { get; set; }
            //public string Receiver { get; set; }
            public string UserType { get; set; }
            public Guid? UserId { get; set; }

            //public string Support {  get; set; }

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
                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernQuery.First().RequestorByUser.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    var fillterApproval = ticketConcernQuery.Select(x => x.RequestTransactionId);

                    var allUserList = await _context.UserRoles.ToListAsync();

                    var receiverPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver)).Select(x => x.UserRoleName).ToList();

                    var issueHandlerPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.IssueHandler)).Select(x => x.UserRoleName).ToList();

                    var supportPermissionList = allUserList.Where(x => x.Permissions
                    .Contains(TicketingConString.Support)).Select(x => x.UserRoleName).ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.User.Fullname.Contains(request.Search)
                        || x.User.SubUnit.SubUnitName.Contains(request.Search));

                    }

                    if (request.Status != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == true);
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
                                var channelUserValidation = await _context.ChannelUsers.Where(x => x.UserId == request.UserId).ToListAsync();
                                var channelSelectValidation = channelUserValidation.Select(x => x.ChannelId);
                                ticketConcernQuery = ticketConcernQuery.Where(x => channelSelectValidation.Contains(x.ChannelId.Value));
                            }
                            else
                            {
                                ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                            }

                        }


                        if (request.UserType == TicketingConString.Receiver && receiverList != null)
                        {
                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && request.UserId == receiverList.UserId)
                            {

                                var receiver = await _context.TicketConcerns.Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId).ToListAsync();
                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                var requestorSelect = receiver.Select(x => x.RequestTransactionId);

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => receiverContains.Contains(x.RequestorByUser.BusinessUnitId) && requestorSelect.Contains(x.RequestTransactionId));


                            }
                            else
                            {
                                ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                            }

                        }
                        else
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestTransactionId == null);
                        }
                    }

                }

                var results = ticketConcernQuery.Where(x => x.IsApprove != false && x.IsReTicket != true)
                    .GroupBy(x => new
                    {
                        x.RequestTransactionId,
                        x.UserId,
                        IssueHandler = x.User.Fullname

                    }).Select(x => new GetOpenTicketResult
                    {
                        CloseTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove == true),
                        RequestedTicketCount = ticketConcernQuery.Count(),
                        OpenTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove != true),
                        DelayedTicketCount = ticketConcernQuery.Count(x => x.TargetDate < dateToday && x.IsClosedApprove != true),
                        RequestTransactionId = x.Key.RequestTransactionId,
                        Description = x.First().ConcernDetails,
                        DepartmentId = x.First().RequestorByUser.DepartmentId,
                        Department_Name = x.First().RequestorByUser.Department.DepartmentName,
                        Requestor_By = x.First().RequestorBy,
                        Requestor_Name = x.First().RequestorByUser.Fullname,
                        UserId = x.Key.UserId,
                        Issue_Handler = x.Key.IssueHandler,
                        UnitId = x.First().User.UnitId,
                        Unit_Name = x.First().User.Units.UnitName,
                        SubUnitId = x.First().User.SubUnitId,
                        SubUnit_Name = x.First().User.SubUnit.SubUnitName,
                        ProjectId = x.First().Channel.ProjectId,
                        Project_Name = x.First().Channel.Project.ProjectName,
                        ChannelId = x.First().ChannelId,
                        Channel_Name = x.First().Channel.ChannelName,
                        Concern_Type = x.First().TicketType,
                        OpenTickets = x.Select(x => new OpenTicket
                        {

                            TicketConcernId = x.Id,
                            RequestConcernId = x.RequestConcernId,
                            Concern_Description = x.ConcernDetails,

                            Ticket_Status = x.IsApprove == true && x.IsReTicket != false && x.IsTransfer != false && x.IsClosedApprove != false ? TicketingConString.OpenTicket
                                    : x.IsTransfer == false ? TicketingConString.ForTransfer : x.IsReTicket == false ? TicketingConString.ForReticket : x.IsClosedApprove == false ? TicketingConString.ForClosing : "Unknown",

                            Category_Description = x.Category.CategoryDescription,
                            SubCategory_Description = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Remarks = x.Remarks,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            IsActive = x.IsActive,

                        }).ToList()

                    });

                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
