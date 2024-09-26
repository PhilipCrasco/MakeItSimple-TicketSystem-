using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int ? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }

        public string ConcernDetails { get; set; }

        public int ? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ? SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public bool ? IsTransfer { get; set; }
        public DateTime? TransferAt { get; set; }
        public Guid? TransferBy { get; set; }
        public virtual User TransferByUser { get; set; }

        public bool ? IsApprove { get; set; } 
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public virtual User ApprovedByUser { get; set; }

        public bool ? IsClosedApprove { get; set; }
        public DateTime? Closed_At { get; set; }
        public Guid? ClosedApproveBy { get; set; }
        public virtual User ClosedApproveByUser { get; set; }

        public bool IsClosedReject { get; set; } = false;
        public bool IsReject { get; set; } = false;

        public bool ? IsReTicket { get; set; }
        public DateTime ? ReticketAt { get; set; }
        public Guid ? ReticketBy { get; set; }
        public virtual User ReticketByUser { get; set; }

        public string Remarks { get; set; }

        public DateTime ? StartDate { get; set; }

        public DateTime ? TargetDate { get; set; }

        public bool ? IsDone { get; set; }

        public string ConcernStatus { get; set; }

        public string TicketType { get; set; }

        public Guid? RequestorBy { get; set; }

        public virtual User RequestorByUser { get; set; }

        public int ? RequestConcernId { get; set; }

        public virtual RequestConcern RequestConcern { get; set; }

        public Guid? TicketApprover { get; set; }

        public bool ? IsAssigned { get; set; }

        public string TicketNo { get; set; }

        public bool? IsReDate { get; set; }
        public DateTime? ReDateAt { get; set; }
        public Guid? ReDateBy { get; set; }
        public virtual User ReDateByUser { get; set; }

        public ICollection<TicketAttachment> TicketAttachments { get; set; }    

        public ICollection<ClosingTicket> ClosingTickets { get; set; }
        public ICollection<TransferTicketConcern> TransferTicketConcerns { get; set; }

        public ICollection<TicketComment> TicketComments { get; set; }
        public ICollection<TicketCommentView> TicketCommentViews { get; set; }

        public ICollection<TicketHistory> ticketHistories { get; set; }



    }
}
