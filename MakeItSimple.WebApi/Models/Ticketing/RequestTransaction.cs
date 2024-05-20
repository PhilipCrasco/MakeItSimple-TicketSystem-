using System.Collections.ObjectModel;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class RequestTransaction
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } =  true;                                              

        public ICollection<TicketConcern> TicketConcerns { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<TransferTicketConcern> TransferTicketConcerns { get; set; }
        public ICollection<ApproverTicketing> ApproverTicketings { get; set; }
        public ICollection<ReTicketConcern> ReTicketConcerns { get; set; }
        public ICollection<TicketHistory> TicketHistories  { get; set; }
        public ICollection<TicketComment> TicketComments { get; set; }

    }
}
