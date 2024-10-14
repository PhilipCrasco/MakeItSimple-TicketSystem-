using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest
{
    public partial class AddRequestConcern
    {
        public class AddRequestConcernCommand : IRequest<Result>
        {

            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int? RequestConcernId { get; set; }
            public int? CompanyId { get; set; }
            public int? BusinessUnitId { get; set; }
            public int? DepartmentId { get; set; }
            public int? UnitId { get; set; }
            public int? SubUnitId { get; set; }
            public int? LocationId { get; set; }
            public int? ChannelId { get; set; }
            public int? CategoryId { get; set; }
            public int? SubCategoryId { get; set; }

            public DateTime? DateNeeded { get; set; }
            public Guid? UserId { get; set; }

            public string Concern { get; set; }
            public string Remarks { get; set; }
            public string Notes { get; set; }
            public int? Contact_Number { get; set; }
            public string Request_Type { get; set; }

            public List<RequestAttachmentsFile> RequestAttachmentsFiles { get; set; }

            public class RequestAttachmentsFile
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }

        }
    }
}
