using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport
{
    public partial class OnHoldTicketReport
    {

        public class Handler : IRequestHandler<OnHoldTicketReportQuery, PagedList<OnHoldTicketReportResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<OnHoldTicketReportResult>> Handle(OnHoldTicketReportQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TicketOnHold> query = _context.TicketOnHolds
                    .Include(q => q.TicketConcern)
                    .ThenInclude(q => q.RequestConcern)
                    .Include(q => q.AddedByUser);

                if (request.Unit is not null)
                {
                    query = query.Where(x => x.AddedByUser.UnitId == request.Unit);

                    if (request.UserId is not null)
                    {
                        query = query.Where(x => x.AddedBy == request.UserId);
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.AddedByUser.Fullname.Contains(request.Search));
                }

                var reports = query
                    .Select(r => new OnHoldTicketReportResult
                    {
                        TicketConcernId = r.TicketConcernId.Value,
                        Concerns = r.TicketConcern.RequestConcern.Concern,
                        Reason = r.Reason,
                        Added_By = r.AddedByUser.Fullname,
                        Created_At = r.CreatedAt,
                        IsHold = r.IsHold,
                        Resume_At = r.ResumeAt,
                    });


                return await PagedList<OnHoldTicketReportResult>.CreateAsync(reports, request.PageNumber , request.PageSize);

            }
        }
    }
}
