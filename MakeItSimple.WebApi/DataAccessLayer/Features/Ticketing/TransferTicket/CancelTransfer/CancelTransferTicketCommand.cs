using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket
{
    public partial class CancelTransferTicket
    {
        public class CancelTransferTicketCommand : IRequest<Result>
        {

            public int TransferTicketId { get; set; }
            public Guid ? Transacted_By {  get; set; }


        }
    }
}
