using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ViewImage
{
    public partial class ViewTicketImage
    {
        public class ViewTicketImageCommand : IRequest<Result>
        {
            public int TicketAttachmentId { get; set; }
        }
    }
}
