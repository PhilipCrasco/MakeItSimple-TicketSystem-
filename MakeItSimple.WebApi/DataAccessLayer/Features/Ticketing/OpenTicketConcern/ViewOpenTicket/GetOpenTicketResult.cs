using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    public partial class GetOpenTicket
    {
        public class GetOpenTicketResult
        {
            public int? TicketConcernId { get; set; }
            public int? RequestConcernId { get; set; }
            public string Concern_Description { get; set; }

            public string Company_Code { get; set; }
            public string Company_Name { get; set; }

            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

            public string Department_Code { get; set; }
            public string Department_Name { get; set; }

            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }

            public string Location_Code { get; set; }
            public string Location_Name { get; set; }

            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }

            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public DateTime? Date_Needed { get; set; }
            public string Notes { get; set; }
            public int? Contact_Number { get; set; }
            public string Request_Type { get; set; }

            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Issue_Handler { get; set; }
            public DateTime? Target_Date { get; set; }

            public string Ticket_Status { get; set; }
            public string Concern_Type { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool IsActive { get; set; }
            public string Remarks { get; set; }
            public bool? Done { get; set; }

            public bool? Is_Transfer { get; set; }
            public DateTime? Transfer_At { get; set; }
            public bool? Is_Closed { get; set; }
            public DateTime? Closed_At { get; set; }

            public string Closed_Status { get; set; }

            public List<GetForClosingTicket> GetForClosingTickets { get; set; }
            public List<GetForTransferTicket> GetForTransferTickets { get; set; }

            public List<TransferApprovalTicket> TransferApprovalTickets { get; set; }

            public List<GetOnHold> GetOnHolds { get; set; }

            public class GetForClosingTicket
            {
                public int? ClosingTicketId { get; set; }
                public string Resolution { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
                public string Notes { get; set; }
                public string Remarks { get; set; }
                public bool? IsApprove { get; set; }
                public string Approver { get; set; }

                public List<ApproverList> ApproverLists { get; set; }

                public class ApproverList
                {
                    public string ApproverName { get; set; }
                    public int Approver_Level { get; set; }
                }

                public List<GetAttachmentForClosingTicket> GetAttachmentForClosingTickets { get; set; }
                public class GetAttachmentForClosingTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            }

            public class GetForTransferTicket
            {
                public int? TransferTicketConcernId { get; set; }
                public string Transfer_Remarks { get; set; }
                public bool? IsApprove { get; set; }

                public List<GetAttachmentForTransferTicket> GetAttachmentForTransferTickets { get; set; }
                public class GetAttachmentForTransferTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            }

            public class TransferApprovalTicket
            {
                public int? TransferTicketConcernId { get; set; }
                public string Transfer_Remarks { get; set; }
                public bool? IsApprove { get; set; }

                public List<GetAttachmentTransferApprovalTicket> GetAttachmentTransferApprovalTickets { get; set; }
                public class GetAttachmentTransferApprovalTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            }

            public class GetOnHold
            {
                public int Id { get; set; }
                public string Reason { get; set; }
                public string AddedBy { get; set; }
                public DateTime CreatedAt { get; set; }
                public bool? IsHold { get; set; }
                public DateTime? ResumeAt { get; set; }

                public List<GetAttachmentForOnHoldTicket> GetAttachmentForOnHoldTickets { get; set; }
                public class GetAttachmentForOnHoldTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }



            }

            public DateTime ? Transaction_Date { get; set; }

        }
    }
}
