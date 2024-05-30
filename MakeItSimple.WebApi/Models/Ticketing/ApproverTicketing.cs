using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ApproverTicketing
    {
        
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public int ? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public int ? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public bool ? IsApprove { get; set; }
        public int? ApproverLevel { get; set; }

        //public Guid? IssueHandler { get; set; }

        public int ? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }

        public string Status { get; set; }

        public int ? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }

        public int ? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public int ? TransferTicketConcernId { get; set; }
        public virtual TransferTicketConcern TransferTicketConcern { get; set; }

        public int ? TicketReDateId { get; set; }
        public virtual TicketReDate TicketReDate { get; set; }

        public int? ReTicketConcernId { get; set; }
        public virtual ReTicketConcern ReTicketConcern { get; set; }



    }
}
