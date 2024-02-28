using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern
{
    public class GetOpenTicket
    {

        public class GetOpenTicketResult
        {
            public int TicketConcernId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string TicketStatus { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public DateTime Start_Date { get; set; }
            public DateTime Target_Date { get; set; }

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
                    .Include(x => x.Department)
                    .Include(x => x.SubUnit)
                    .Include(x => x.Channel)
                    .Include(x => x.User)
                    .Include(x => x.Category)
                    .Include(x => x.SubCategory);

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

                var results = ticketConcernQuery.Where(x => x.IsApprove == true
                && x.IsClosedApprove != true).OrderBy(x => x.Id)
                    .Select(x => new GetOpenTicketResult
                    {
                        TicketConcernId = x.Id,
                        Department_Code = x.Department.DepartmentName,
                        Department_Name = x.Department.DepartmentName,
                        Unit_Code = x.Unit.UnitCode,
                        Unit_Name = x.Unit.UnitName,
                        SubUnit_Code = x.SubUnit.SubUnitCode,
                        SubUnit_Name = x.SubUnit.SubUnitName,
                        Channel_Name = x.Channel.ChannelName,
                        EmpId = x.User.EmpId,
                        Fullname = x.User.Fullname,
                        TicketStatus = x.IsApprove == true && x.IsReTicket != false && x.IsTransfer != false && x.IsClosedApprove != false ? "Open Ticket"
                        : x.IsTransfer == false ? "For Approval Transfer" : x.IsReTicket == false ? "For ReTicket" : x.IsClosedApprove == false ? "For Closing Ticket" : "Unknown", 
                        Category_Description = x.Category.CategoryDescription,
                        SubCategory_Description = x.SubCategory.SubCategoryDescription,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate

                    });


                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
