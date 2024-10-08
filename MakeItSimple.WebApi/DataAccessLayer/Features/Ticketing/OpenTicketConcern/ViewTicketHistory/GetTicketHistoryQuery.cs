using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewTicketHistory
{

    public partial class GetTicketHistory
    {
        public class GetTicketHistoryQuery : IRequest<Result>
        {
            public int TicketConcernId { get; set; }
        }

    }
}
