using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;

namespace MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup
{
    public class BusinessUnit : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public DateTime SyncDate { get; set; }
        public string SyncStatus { get; set; }

        public int ? Business_No { get; set; }
        public string BusinessCode { get; set; }
        public string BusinessName { get; set; }

        public int ? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
