namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketCommentView
    {

        public int Id { get; set; }
        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }
        public bool ? IsClicked { get; set; }
        public int? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }

        public int? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }


        public int ? TicketCommentId { get; set; }
        public virtual TicketComment TicketComment { get; set; }

    }
}
