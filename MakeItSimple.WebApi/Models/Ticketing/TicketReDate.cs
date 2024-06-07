namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketReDate
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        
        public int ? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public int? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }

        public int? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }

        public string Concern_Description { get; set; }

        public Guid ? UserId { get; set; }

        public DateTime ? StartDate { get; set; }

        public DateTime ? TargetDate { get; set; }

        public bool? IsReDate { get; set; }
        public DateTime? ReDateAt { get; set; }
        public Guid? ReDateBy { get; set; }
        public string ReDateRemarks { get; set; }
        public virtual User ReDateByUser { get; set; }

        public bool IsRejectReDate { get; set; }
        public DateTime? RejectReDateAt { get; set; }
        public Guid? RejectReDateBy { get; set; }
        public virtual User RejectReDateByUser { get; set; }
        public string RejectRemarks { get; set; }
        public string Remarks { get; set; }
        public Guid? TicketApprover { get; set; }

        public ICollection<ApproverTicketing> ApproverTickets { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }



    }
}
