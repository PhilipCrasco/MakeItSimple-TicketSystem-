namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketComment
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public string Comment { get; set; }
        public int? RequestGeneratorId { get; set; }
        public virtual RequestGenerator RequestGenerator { get; set; }

        //public int? TicketConcernId { get; set; }
        //public virtual TicketConcern TicketConcern { get; set; }
        public bool? IsClicked { get; set; }


        public ICollection<TicketCommentView>  TicketCommentViews { get; set; }

    }
}
