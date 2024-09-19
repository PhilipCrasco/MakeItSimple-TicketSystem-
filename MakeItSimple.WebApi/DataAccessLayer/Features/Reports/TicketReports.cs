using Humanizer;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports
{
    public class TicketReports
    {

        public record class Reports
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime Start_Date { get; set; }
            public DateTime End_Date { get; set; }
            public string Personnel { get; set; }
            public int Ticket_Number { get; set; }
            public string Description { get; set; }
            public DateTime Target_Date { get; set; }
            public DateTime Actual { get; set; }
            public int Varience {get; set;}
            public decimal ? Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }

        }

        public class TicketReportsQuery : UserParams , IRequest<PagedList<Reports>>
        {
            public int ? Department { get; set; }
            public Guid ? UserId { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }

        }


        public class Handler : IRequestHandler<TicketReportsQuery, PagedList<Reports>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<Reports>> Handle(TicketReportsQuery request, CancellationToken cancellationToken)
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


                

                var results = ticketQuery
                    .Select(x => new Reports
                    {
                        Year = x.TargetDate.Value.Date.Year,
                        Month = x.TargetDate.Value.Date.Month,                
                        Start_Date = new DateTime(x.TargetDate.Value.Date.Month, 1 , x.TargetDate.Value.Date.Year),
                        End_Date = new DateTime(x.TargetDate.Value.Date.Month, 
                        DateTime.DaysInMonth(x.TargetDate.Value.Date.Year , x.TargetDate.Value.Date.Month), x.TargetDate.Value.Date.Year),
                        Personnel = x.User.Fullname,
                        Ticket_Number = x.Id,
                        Description = x.ConcernDetails,
                        Target_Date = new DateTime(x.TargetDate.Value.Date.Month, x.TargetDate.Value.Date.Day, x.TargetDate.Value.Date.Year),
                        Actual = x.Closed_At != null ? new DateTime(x.Closed_At.Value.Date.Month, x.TargetDate.Value.Date.Day, x.TargetDate.Value.Date.Year)
                        : new DateTime(x.TargetDate.Value.Date.Month, x.TargetDate.Value.Date.Day, x.TargetDate.Value.Date.Year),
                       Varience = EF.Functions.DateDiffDay(x.TargetDate.Value.Date , x.Closed_At.Value.Date),
                       Efficeincy = x.Closed_At != null ? Math.Max(0,100m - ((decimal)EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date)
                       / DateTime.DaysInMonth(x.TargetDate.Value.Date.Year, x.TargetDate.Value.Date.Month) * 100m)) : null,
                       Status = x.Closed_At != null ? TicketingConString.Closed : TicketingConString.OpenTicket,
                       Remarks = x.Closed_At == null ? null : x.TargetDate.Value > x.Closed_At.Value ? TicketingConString.Delay : TicketingConString.OnTime
                    });



                return await PagedList<Reports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }

    }
}
