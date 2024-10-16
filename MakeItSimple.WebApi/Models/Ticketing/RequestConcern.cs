using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class RequestConcern : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public string ConcernStatus { get; set; }

        public bool IsReject { get; set; } = false;
        public Guid? RejectBy { get; set; }
        public virtual User RejectByUser { get; set; }

        public bool ? IsDone { get; set; }

        public string Concern { get; set; }
        public string Remarks { get; set; }
        public string Resolution { get; set; }

        public bool ? Is_Confirm { get; set; }
        public DateTime? Confirm_At { get; set; }

        public int ? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int ? BusinessUnitId { get; set; }
        public virtual BusinessUnit BusinessUnit { get; set; }

        public int ? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int ? UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public int ? SubUnitId {  get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int? SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public int ? LocationId { get; set; }
        public virtual Location Location { get; set; }

        public DateTime? DateNeeded { get; set; }

        public string Notes { get; set; }


        public string ContactNumber { get; set; }

        public string RequestType { get; set; }

        public ICollection<TicketConcern> TicketConcerns { get; set; }




    }
}
