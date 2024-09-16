namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketTransactionNotification
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public Guid AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public DateTime Created_At { get; set; }

        public Guid ReceiveBy { get; set; }
        public virtual User ReceiveByUser { get; set; }

        public string Modules { get; set; }



    }
}
