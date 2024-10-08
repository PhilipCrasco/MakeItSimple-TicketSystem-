using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RemoveTicketAttachment
{
    public partial class RemoveTicketAttachment
    {
        public class RemoveTicketAttachmentCommand : IRequest<Result>
        {
            public int? TicketAttachmentId { get; set; }
        }
    }
}
