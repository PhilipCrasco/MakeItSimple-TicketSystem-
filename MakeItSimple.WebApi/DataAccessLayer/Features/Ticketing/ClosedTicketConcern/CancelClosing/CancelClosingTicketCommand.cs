using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.CancelClosing
{
    public partial class CancelClosingTicket
    {
        public class CancelClosingTicketCommand : IRequest<Result>
        {
            public Guid? Transacted_By { get; set; }
            public int? ClosingTicketId { get; set; }

            public string Modules { get; set; }
        }
    }
}
