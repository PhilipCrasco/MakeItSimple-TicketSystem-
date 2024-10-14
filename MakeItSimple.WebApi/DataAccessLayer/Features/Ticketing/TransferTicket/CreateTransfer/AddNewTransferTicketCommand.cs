using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CreateTransfer
{
    public partial class AddNewTransferTicket
    {
        public class AddNewTransferTicketCommand : IRequest<Result>
        {
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public Guid? Transfer_By { get; set; }
            public Guid ? Transfer_To { get; set; }
            public DateTime? Current_Target_Date { get; set; }
            public int? TransferTicketId { get; set; }
            public int? TicketConcernId { get; set; }
            public string TransferRemarks { get; set; }
            public List<AddTransferAttachment> AddTransferAttachments { get; set; }

            public class AddTransferAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
        }

    }
}
