using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport
{
    public partial class OpenTicketReports
    {
        public class OpenTicketReportsQuery : UserParams, IRequest<PagedList<OpenTicketReportsResult>>
        {

            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }
    }
}
