namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketOnHold
    {

        public int Id { get; set; }

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Reason { get; set; }
        public bool ? IsHold { get; set; }
        public DateTime? ResumeAt { get; set; }

        public int? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public ICollection<TicketAttachment> TicketAttachments { get; set; }

    }
}
