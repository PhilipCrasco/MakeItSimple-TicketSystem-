using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using System.ComponentModel.DataAnnotations.Schema;


namespace MakeItSimple.WebApi.Models.Setup.SubUnitSetup

{
    public partial class SubUnit : BaseEntity
    {

        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int SubUnitNo { get; set; }
        public string SubUnitCode { get; set; }
        public string SubUnitName { get; set; }

        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        //public int ? LocationId { get; set; }
        //public virtual Location Location { get; set; }

        public int? UnitId { get; set; }
        public virtual Unit Unit {get; set;}

        public DateTime SyncDate { get; set; }
        public string SyncStatus { get; set; }


        public ICollection<Channel> Channels { get; set; }
        public ICollection<User> Users { get; set; }

        //public ICollection<Location> Locations { get; set; }

    }

}
