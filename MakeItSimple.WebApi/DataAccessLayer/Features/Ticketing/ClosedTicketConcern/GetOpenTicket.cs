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

            public int ? OpenTicketCount { get; set; }
            public int? RequestGeneratorId { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }

            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }

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
                public Guid? UserId { get; set; }
                public string Issue_Handler { get; set; }
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
            public string Request { get; set; }
            public string Users { get; set; }
            public Guid? UserId { get; set; }
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
                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User);

                if (request.Users == TicketingConString.Users)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.User.Fullname.Contains(request.Search)
                    || x.SubUnit.SubUnitName.Contains(request.Search));

                }

                if (request.Request == TicketingConString.ReTicket)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.IsReTicket == false && x.IsApprove == true);
                }

                else if (request.Request == TicketingConString.Transfer)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.IsTransfer == false && x.IsApprove == true);
                }

                else if (request.Request == TicketingConString.Open)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.IsTransfer != false && x.IsReTicket != false && x.IsApprove == true);
                }

                if (request.Status != null)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == true);
                }

                var results = ticketConcernQuery.Where(x => x.IsClosedApprove != true && x.IsApprove != false)
                    .GroupBy(x => x.RequestGeneratorId).Select(x => new GetOpenTicketResult
                {

                    OpenTicketCount = ticketConcernQuery.Count(x => x.Id == ticketConcernQuery.First().Id),
                    
                        RequestGeneratorId = x.Key,
                        DepartmentId = x.First().RequestorByUser.DepartmentId,
                        Department_Name = x.First().RequestorByUser.Department.DepartmentName,
                        Requestor_By = x.First().RequestorBy,
                        Requestor_Name = x.First().RequestorByUser.Fullname,
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
                            UserId = x.UserId,
                            Issue_Handler = x.User.Fullname,
                            Concern_Description = x.ConcernDetails,
                            Ticket_Status = x.IsApprove == true && x.IsReTicket != false && x.IsTransfer != false && x.IsClosedApprove != false ? "Open Ticket"
                                    : x.IsTransfer == false ? "For Approval Transfer" : x.IsReTicket == false ? "For ReTicket" : x.IsClosedApprove == false ? "For Closing Ticket" : "Unknown",
                            Category_Description = x.Category.CategoryDescription,
                            SubCategory_Description = x.SubCategory.SubCategoryDescription,
                            Start_Date = x.StartDate,
                            Target_Date = x.TargetDate,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            IsActive = x.IsActive,

                        }).ToList()
                });



                //OpenTicketByIds = group.Select(x => new OpenTicketById
                //{

                //}).ToList()


                //{




                //}).ToList(),

                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
