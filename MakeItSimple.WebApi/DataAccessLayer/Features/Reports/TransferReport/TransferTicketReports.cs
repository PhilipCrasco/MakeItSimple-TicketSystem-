using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport
{
    public partial class TransferTicketReports
    {

         public class Handler : IRequestHandler<TransferTicketReportsQuery, PagedList<TransferTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<TransferTicketReportsResult>> Handle(TransferTicketReportsQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TransferTicketConcern> _transferQuery = _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser)
                    .Include(x => x.TransferToUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestConcern);


                if (request.Unit is not null)
                {
                    _transferQuery = _transferQuery.Where(x => x.TransferByUser.UnitId == request.Unit);

                    if (request.UserId is not null)
                    {
                        _transferQuery = _transferQuery.Where(x => x.TransferBy == request.UserId);
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    _transferQuery = _transferQuery
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.TransferByUser.Fullname.Contains(request.Search));
                }

                var results = _transferQuery
                    .Where(x => x.IsTransfer == true && x.TicketConcern.UserId != null)
                     .Where(x => x.TransferAt >= request.Date_From && x.TransferAt < request.Date_To)
                    .Select(x => new TransferTicketReportsResult
                    {
                        TicketConcernId = x.TicketConcernId,
                        TransferTicketId = x.Id,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Transfered_By = x.TransferByUser.Fullname,
                        Transfered_To = x.TransferToUser.Fullname,
                        Current_Target_Date = x.Current_Target_Date.Value.Date,
                        Target_Date = x.TicketConcern.TargetDate,
                        Transfer_At = x.TicketConcern.TransferAt,
                        Transfer_Remarks = x.TransferRemarks,
                        Remarks = x.TransferRemarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,

                    });


                return await PagedList<TransferTicketReportsResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
