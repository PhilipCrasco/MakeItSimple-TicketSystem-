using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket
{
    public partial class RequestApprovalReceiver
    {
        public class RequestApprovalReceiverCommand : IRequest<Result>
        {
            public Guid? Approved_By { get; set; }
            public Guid? UserId { get; set; }
            public string Role { get; set; }
            public int? TicketConcernId { get; set; }

        }
    }
}
