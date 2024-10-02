using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports
{
    public class TransferTicketReports
    {
       
        public class TransferTicketReportsResult
        {
            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public string Concern_Details { get; set; }
            public string Transfer_By { get; set; }
            public string Transfer_To { get; set; }
            public string Channel_Name { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime? Transfer_At { get; set; }
            public string Transfer_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

        }

        public class TransferTicketReportsQuery : UserParams, IRequest<PagedList<TransferTicketReportsResult>>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            [Required]
            public DateTime? Date_From { get; set; }
            [Required]
            public DateTime? Date_To { get; set; }

        }

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
                    .AsNoTracking()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Include(x => x.TicketConcern);


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
                        Concern_Details = x.TicketConcern.ConcernDetails,
                        Transfer_By  = x.TransferByUser.Fullname,
                        Transfer_To = x.TicketConcern.User.Fullname,
                        Category_Description = x.TicketConcern.Category.CategoryDescription,
                        SubCategory_Description = x.TicketConcern.SubCategory.SubCategoryDescription,
                        Start_Date = x.TicketConcern.StartDate, 
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
