using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing
{
    public partial class GetClosingTicket
    {
        public class GetClosingTicketQuery : UserParams, IRequest<PagedList<GetClosingTicketResults>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
            public bool? IsClosed { get; set; }
            public bool? IsReject { get; set; }

        }
    }

}
