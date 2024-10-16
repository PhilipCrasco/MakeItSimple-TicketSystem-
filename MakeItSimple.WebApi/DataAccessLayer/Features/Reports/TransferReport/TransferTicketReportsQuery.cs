using MakeItSimple.WebApi.Common.Pagination;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport
{
    public partial class TransferTicketReports
    {
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
    }
}
