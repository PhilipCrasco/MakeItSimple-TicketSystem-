namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int? RequestTransactionId { get; set; }
        public virtual RequestTransaction RequestTransaction { get; set; }
        public int? TicketTransactionId { get; set; }
        public virtual TicketTransaction TicketTransaction { get; set; }
        public Guid ? RequestorBy { get; set; }
        public virtual User RequestorByUser { get; set; }
        public Guid ? ApproverBy { get; set; }
        public virtual User ApproverByUser { get; set; }
        public string Request {  get; set; }
        public string Status { get; set; }
        public DateTime ? TransactionDate { get; set; }

        public int ? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

    }

}
