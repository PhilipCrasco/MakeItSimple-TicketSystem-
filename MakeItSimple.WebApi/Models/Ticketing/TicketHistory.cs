namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int ? RequestGeneratorId { get; set; }   
        public virtual RequestGenerator RequestGenerator { get; set; }
        public Guid ? RequestorBy { get; set; }
        public virtual User RequestorByUser { get; set; }
        public Guid ? ApproverBy { get; set; }
        public virtual User ApproverByUser { get; set; }
        public string Request {  get; set; }
        public string Status { get; set; }
        public DateTime ? TransactionDate { get; set; }


    }

}
