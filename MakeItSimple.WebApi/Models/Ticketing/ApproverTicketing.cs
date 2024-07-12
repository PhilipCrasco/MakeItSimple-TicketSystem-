

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ApproverTicketing
    {
        
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public bool ? IsApprove { get; set; }
        public int? ApproverLevel { get; set; }
        public string Status { get; set; }

        public int ? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public int ? TransferTicketConcernId { get; set; }
        public virtual TransferTicketConcern TransferTicketConcern { get; set; }


        public int ? ClosingTicketId { get; set; }
        public virtual ClosingTicket ClosingTicket { get; set; }



    }
}
