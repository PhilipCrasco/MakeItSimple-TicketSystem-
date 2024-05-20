namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketTransaction
    {
        public int Id { get; set; }
        public bool IsActive {get; set; }

        public ICollection<ClosingTicket> ClosingTickets { get; set; }
        public ICollection<ApproverTicketing> ApproverTicketings { get; set; }

        public ICollection<TicketHistory> TicketHistories { get; set; }
        public ICollection<TransferTicketConcern> TransferTicketConcerns { get; set; }

        public ICollection<ReTicketConcern> ReTicketConcerns { get;set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }
    }
}
