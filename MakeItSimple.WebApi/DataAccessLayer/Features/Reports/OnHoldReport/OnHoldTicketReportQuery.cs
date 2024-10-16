using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport
{
    public partial class OnHoldTicketReport
    {
        public class OnHoldTicketReportQuery : UserParams, IRequest<PagedList<OnHoldTicketReportResult>>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }
    }
}
