using System.Collections.ObjectModel;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class RequestGenerator
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<TicketConcern> TicketConcerns { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<TransferTicketConcern> TransferTicketConcerns { get; set; }
        public ICollection<ApproverTicketing> ApproverTicketings { get; set; }

    }
}
