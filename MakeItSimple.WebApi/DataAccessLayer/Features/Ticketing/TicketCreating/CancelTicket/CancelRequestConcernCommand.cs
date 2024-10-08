using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicket
{
    public partial class CancelRequestConcern
    {
        public class CancelRequestConcernCommand : IRequest<Result>
        {

            public int? RequestConcernId { get; set; }

        }
    }
}
