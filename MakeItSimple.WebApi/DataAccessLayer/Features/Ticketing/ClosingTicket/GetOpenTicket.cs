using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosingTicket
{
    public class GetOpenTicket
    {

        public class GetOpenTicketResult
        {
            public int TicketConcernId {  get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
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


        public class GetOpenTicketQuery : UserParams , IRequest<PagedList<GetOpenTicketResult>>
        {
            public bool ? Status { get; set; }
            public string Search { get; set; }

            public Guid ? UserId { get; set; }
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



                var channeluserExist = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                if (channeluserExist != null)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.User.Fullname == channeluserExist.Fullname);
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                   ticketConcernQuery = ticketConcernQuery.Where(x => x.User.Fullname.Contains(request.Search) 
                   || x.SubUnit.SubUnitName.Contains(request.Search));

                }

                if(request.Status != null)
                {
                    ticketConcernQuery = ticketConcernQuery.Where(x => x.IsActive == true);
                }

                var results = ticketConcernQuery.Where(x => x.IsApprove == true 
                && x.IsClosedApprove == null).OrderBy(x => x.Id)
                    .Select(x => new GetOpenTicketResult
                    {
                        TicketConcernId = x.Id,
                        Department_Code = x.Department.DepartmentName,
                        Department_Name = x.Department.DepartmentName,
                        SubUnit_Code = x.SubUnit.SubUnitCode,
                        SubUnit_Name = x.SubUnit.SubUnitName,   
                        Channel_Name = x.Channel.ChannelName,
                        EmpId = x.User.EmpId,
                        Fullname = x.User.Fullname,
                        TicketStatus = x.IsApprove == true && x.IsReTicket != false && x.IsTransfer != false ? "Open Ticket" 
                        : x.IsTransfer == false ? "For Approval Transfer" : x.IsReTicket == false ? "For ReTicket" : "Unknown",
                        Category_Description = x.Category.CategoryDescription,
                        SubCategory_Description = x.SubCategory.SubCategoryDescription,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate

                    });
               
               
                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber , request.PageSize);
            }
        }
    }
}
