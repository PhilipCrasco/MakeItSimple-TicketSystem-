namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ClosingGenerator 
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public ICollection<TicketConcern> TicketConcerns { get; set; }
        public ICollection<ClosingTAttachment> TicketAttachments { get; set; }

    }
}
