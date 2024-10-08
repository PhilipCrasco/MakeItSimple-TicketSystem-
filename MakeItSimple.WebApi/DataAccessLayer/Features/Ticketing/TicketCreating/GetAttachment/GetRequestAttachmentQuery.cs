using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetAttachment
{
    public partial class GetRequestAttachment
    {
        public class GetRequestAttachmentQuery : IRequest<Result>
        {
            public int? Id { get; set; }
            public bool? Status { get; set; }
        }
    }
}
