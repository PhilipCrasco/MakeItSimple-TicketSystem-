namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketGenerator
    {
        public int Id { get; set; }
        public bool IsActive {get; set; }

        public ICollection<ClosingTicket> ClosingTickets { get; set; }
        public ICollection<ApproverTicketing> ApproverTicketings { get; set; }

        public ICollection<TicketHistory> TicketHistories { get; set; }
    }
}
