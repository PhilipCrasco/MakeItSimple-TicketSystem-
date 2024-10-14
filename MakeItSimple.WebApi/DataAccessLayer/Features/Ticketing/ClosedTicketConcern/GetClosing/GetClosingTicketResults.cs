namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing
{
    public partial class GetClosingTicket
    {
        public class GetClosingTicketResults
        {
            public int ClosingTicketId { get; set; }
            public int TicketConcernId { get; set; }
            public string Resolution { get; set; }
            public string Notes { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }
            public string Category_Description { get; set; }
            public string SubCategoryDescription { get; set; }
            public int? Delay_Days { get; set; }
            public string Closed_By { get; set; }
            public DateTime? Closed_At { get; set; }
            public string Closed_Status { get; set; }
            public string Closed_Remarks { get; set; }
            public string RejectClosed_By { get; set; }
            public DateTime? RejectClosed_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }

            public List<ClosingAttachment> ClosingAttachments { get; set; }

            public class ClosingAttachment
            {
                public int? TicketAttachmentId { get; set; }
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
