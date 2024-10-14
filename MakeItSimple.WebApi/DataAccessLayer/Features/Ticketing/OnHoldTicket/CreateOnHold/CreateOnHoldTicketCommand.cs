using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.CreateOnHold
{
    public partial class CreateOnHoldTicket
    {
        public class CreateOnHoldTicketCommand : IRequest<Result>
        {
            public int? Id { get; set; }
            public int TicketConcernId { get; set; }
            public Guid? Added_By { get; set; }
            public string Reason { get; set; }
            public List<OnHoldAttachment> OnHoldAttachments { get; set; }
            public class OnHoldAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }

            }

        }
    }
}
