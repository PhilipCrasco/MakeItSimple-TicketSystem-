using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer
{
    public partial class ApprovedTransferTicket
    {
        public class ApprovedTransferTicketCommand : IRequest<Result>
        {
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public int TransferTicketId { get; set; }

            public DateTime ? Target_Date { get; set; }

        }
    }
}
