using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ReTicketConcern : BaseEntity
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

        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int? UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public int SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public string ConcernDetails { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }

        public bool? IsReTicket { get; set; }
        public DateTime? ReTicketAt { get; set; }
        public Guid? ReTicketBy { get; set; }
        public string ReTicketRemarks { get; set; }
        public virtual User ReTicketByUser { get; set; }

        public bool IsRejectReTicket { get; set; }

        public DateTime? RejectReTicketAt { get; set; }
        public Guid? RejectReTicketBy { get; set; }
        public virtual User RejectReTicketByUser { get; set; }
        public string RejectRemarks { get; set; }
        public Guid? TicketApprover { get; set; }

        public int? RequestGeneratorId { get; set; }
        public virtual RequestGenerator RequestGenerator { get; set; }



    }
}
