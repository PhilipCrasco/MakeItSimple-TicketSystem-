﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports
{
    public class OpenTicketReports
    {

        public record OpenTicketReportsResult
        {

            public int? TicketConcernId { get; set; }
            public string Concern_Description { get; set; }
            public string Requestor_Name { get; set; }

            public string Issue_Handler { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Name { get; set; }
            public string Channel_Name { get; set; }

            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }

            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string Remarks { get; set; }
        }

        public class OpenTicketReportsQuery : UserParams , IRequest<PagedList<OpenTicketReportsResult>>
        {

            public string Search { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }

        public  class Handler : IRequestHandler<OpenTicketReportsQuery, PagedList<OpenTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<OpenTicketReportsResult>> Handle(OpenTicketReportsQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TicketConcern> ticketQuery = _context.TicketConcerns
                    .AsNoTracking()
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
                    .Include(x => x.RequestConcern);

                if (request.UserId is not null)
                {
                    ticketQuery = ticketQuery.Where(x => x.UserId == request.UserId);
                }

                if (string.IsNullOrEmpty(request.Search))
                {
                    ticketQuery = ticketQuery
                        .Where(x => x.Id.ToString().Contains(request.Search)
                        || x.User.Fullname.Contains(request.Search));
                }

                if (request.Date_From is not null && request.Date_To is not null)
                {
                    ticketQuery = ticketQuery
                        .Where(x => x.TargetDate >= request.Date_From && x.TargetDate < request.Date_To);
                }

                var results = ticketQuery
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.IsTransfer != true)
                    .Select(t => new OpenTicketReportsResult
                    {
                        TicketConcernId = t.Id,
                        Concern_Description = t.ConcernDetails,
                        Requestor_Name = t.RequestorByUser.Fullname,
                        Department_Name = t.RequestorByUser.Department.DepartmentName,
                        Issue_Handler = t.User.Fullname,
                        Unit_Name = t.User.Units.UnitName,
                        SubUnit_Name = t.User.SubUnit.SubUnitName,
                        Channel_Name = t.Channel.ChannelName,
                        Category_Description = t.Category.CategoryDescription,
                        SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        Start_Date = t.StartDate,
                        Target_Date = t.TargetDate,
                        Created_At = t.CreatedAt,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,

                    });


                return await PagedList<OpenTicketReportsResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }

        }
    }
}
 