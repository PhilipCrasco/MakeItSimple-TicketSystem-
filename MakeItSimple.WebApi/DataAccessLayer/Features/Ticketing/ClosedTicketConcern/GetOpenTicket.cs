using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetOpenTicket.GetOpenTicketResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetOpenTicket.GetOpenTicketResult.OpenTicket;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class GetOpenTicket
    {

        public class GetOpenTicketResult
        {
            
            public int ? RequestedTicketCount { get; set; }
            public int ? OpenTicketCount { get; set; }
            public int ? CloseTicketCount { get; set; }
            public int ? DelayedTicketCount { get; set; }
            public int? RequestGeneratorId { get; set; }
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

            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }

            public string Concern_Type { get; set; }

            public List<OpenTicket> OpenTickets { get; set; }
            public class OpenTicket
            {

                public int? TicketConcernId { get; set; }
                public int ? RequestConcernId { get; set; }
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
            public string Receiver { get; set; }
            public string Users { get; set; }
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
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit);
                
                
                if(ticketConcernQuery.Count() > 0)
                {
                    var businessUnitList = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == ticketConcernQuery.First().RequestorByUser.BusinessUnitId);
                    var receiverList = await _context.Receivers.FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitList.Id);
                    var fillterApproval = ticketConcernQuery.Select(x => x.RequestGeneratorId);

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.User.Fullname.Contains(request.Search)
                        || x.User.SubUnit.SubUnitName.Contains(request.Search));

                    }

                    if (request.Status != null)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == true);
                    }

                    if (request.Users == TicketingConString.Users)
                    {
                        ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                    }

                    if (request.Receiver != null)
                    {
                        if (request.Receiver == TicketingConString.Receiver && receiverList != null)
                        {
                            if (request.Role == TicketingConString.Receiver && request.UserId == receiverList.UserId)
                            {

                                var receiver = await _context.TicketConcerns.Where(x => x.RequestorByUser.BusinessUnitId == receiverList.BusinessUnitId).ToListAsync();
                                var receiverContains = receiver.Select(x => x.RequestorByUser.BusinessUnitId);
                                ticketConcernQuery = ticketConcernQuery.Where(x => receiverContains.Contains(x.RequestorByUser.BusinessUnitId));

                            }
                            else
                            {
                                ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestGeneratorId == null);
                            }

                        }
                        else
                        {
                            ticketConcernQuery = ticketConcernQuery.Where(x => x.RequestGeneratorId == null);
                        }

                    }
                }

                var results = ticketConcernQuery.Where(x => x.IsClosedApprove != true && x.IsApprove != false && x.IsReTicket != true)
                    .GroupBy(x => new
                    {
                        x.RequestGeneratorId,
                        x.UserId,
                        IssueHandler = x.User.Fullname

                    }).Select(x => new GetOpenTicketResult
                    {
                        CloseTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove == true),
                        RequestedTicketCount = ticketConcernQuery.Count(),
                        OpenTicketCount = ticketConcernQuery.Count(x => x.IsClosedApprove != true),
                        DelayedTicketCount = ticketConcernQuery.Count(x => x.TargetDate < dateToday && x.IsClosedApprove != true),
                        RequestGeneratorId = x.Key.RequestGeneratorId,
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
