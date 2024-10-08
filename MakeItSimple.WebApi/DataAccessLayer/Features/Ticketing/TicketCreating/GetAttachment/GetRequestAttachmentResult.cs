namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetAttachment
{
    public partial class GetRequestAttachment
    {
        public class GetRequestAttachmentResult
        {
            public int? TicketConcernId { get; set; }

            public List<TicketAttachment> Attachments { get; set; }

            public class TicketAttachment
            {
                public int TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }

                public decimal? FileSize { get; set; }

                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

            }

        }
    }
}
