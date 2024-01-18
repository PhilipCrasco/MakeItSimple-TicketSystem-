using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ClosingTAttachment : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public string ClosingAttachment { get; set; }

        public int? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

    }
}
