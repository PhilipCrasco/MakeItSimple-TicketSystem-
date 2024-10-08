using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.AddClosingTicket
{
    public class AddNewClosingTicketCommand : IRequest<Result>
    {
        public string Closed_Remarks { get; set; }
        public Guid? Modified_By { get; set; }
        public Guid? Added_By { get; set; }
        public int? TicketConcernId { get; set; }
        public int? ClosingTicketId { get; set; }
        public string Resolution { get; set; }
        public string Notes { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Modules { get; set; }
        public List<AddClosingAttachment> AddClosingAttachments { get; set; }

        public class AddClosingAttachment
        {
            public int? TicketAttachmentId { get; set; }
            public IFormFile Attachment { get; set; }

        }
    }

}
