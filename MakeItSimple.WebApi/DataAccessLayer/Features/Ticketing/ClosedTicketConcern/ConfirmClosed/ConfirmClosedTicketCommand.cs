using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ConfirmClosed
{
    public partial class ConfirmClosedTicket
    {
        public class ConfirmClosedTicketCommand : IRequest<Result>
        {
            public int? RequestConcernId { get; set; }
            public Guid? Transacted_By { get; set; }

        }
    }
}
