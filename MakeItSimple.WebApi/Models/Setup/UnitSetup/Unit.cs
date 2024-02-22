using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

namespace MakeItSimple.WebApi.Models.Setup.UnitSetup
{
    public class Unit : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int UnitNo { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public DateTime SyncDate { get; set; }
        public string SyncStatus { get; set; }

        public ICollection<SubUnit> SubUnits { get; set; }
        public ICollection<User> Users { get; set; }

    }
}
