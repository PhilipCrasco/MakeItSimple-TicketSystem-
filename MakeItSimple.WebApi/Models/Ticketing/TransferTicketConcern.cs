using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TransferTicketConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }


        public bool? IsTransfer { get; set; }
        public DateTime? TransferAt { get; set; }
        public Guid? TransferBy { get; set; }
        public string TransferRemarks { get; set; }
        public virtual User TransferByUser { get; set; }

        public bool IsRejectTransfer { get; set; }
        public DateTime? RejectTransferAt { get; set; }
        public Guid? RejectTransferBy { get; set; }
        public virtual User RejectTransferByUser { get; set; }
        public string RejectRemarks { get; set; }
        public Guid? TicketApprover { get; set; }

        public string Remarks { get; set; }
        public string TicketNo { get; set; }

        public ICollection<ApproverTicketing> ApproverTickets { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }

    }



}
