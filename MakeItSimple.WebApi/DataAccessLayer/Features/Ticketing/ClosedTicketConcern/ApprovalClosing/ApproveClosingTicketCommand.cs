using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosing
{
    public partial class ApprovalClosingTicket
    {
        public class ApproveClosingTicketCommand : IRequest<Result>
        {
            public Guid? Closed_By { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Modules { get; set; }
            public List<ApproveClosingRequest> ApproveClosingRequests { get; set; }
            public class ApproveClosingRequest
            {
                public int? ClosingTicketId { get; set; }
            }

        }
    }
}
