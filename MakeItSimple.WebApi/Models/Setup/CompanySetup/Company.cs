using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;

namespace MakeItSimple.WebApi.Models.Setup.CompanySetup
{
    public class Company : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int CompanyNo { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public DateTime SyncDate { get; set; }

        public string SyncStatus { get; set; }

         
        public ICollection<BusinessUnit> BusinessUnits { get; set; }



    }
}
