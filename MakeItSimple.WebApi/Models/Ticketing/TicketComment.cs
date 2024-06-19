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

        public string Attachment { get; set; }
        public string FileName { get; set; }
        public decimal? FileSize { get; set; }

        public int? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }

        public int? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public bool? IsClicked { get; set; }


        public ICollection<TicketCommentView>  TicketCommentViews { get; set; }

    }
}
