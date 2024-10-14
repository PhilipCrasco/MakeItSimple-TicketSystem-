using MakeItSimple.WebApi.Common.Pagination;
using MediatR;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    public partial class GetOpenTicket
    {
        public class GetOpenTicketQuery : UserParams, IRequest<PagedList<GetOpenTicketResult>>
        {
            public string Search { get; set; } 
            public bool? Status { get; set; }
            public string Concern_Status { get; set; }
            public string History_Status { get; set; }
            public string UserType { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }
    }
}
