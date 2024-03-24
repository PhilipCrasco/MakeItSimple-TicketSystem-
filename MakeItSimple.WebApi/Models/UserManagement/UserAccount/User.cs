using MakeItSimple.WebApi.Models.Setup;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public Guid Id { get; set; }
        public bool IsActive { get ; set; } = true;
        public string EmpId {  get; set; } 
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ? IsPasswordChange { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ? UpdatedAt { get; set;}


        [ForeignKey("AddedByUser")]
        public Guid ? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }


        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }

        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int? BusinessUnitId { get; set; }
        public virtual BusinessUnit BusinessUnit { get; set; }

        //public int ? ReceiverId { get; set; }
        //public virtual Receiver Receiver { get; set; }


        public int ? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int? UnitId { get; set; }
        public virtual Unit Units { get; set; }

        public int ? SubUnitId { get; set; }
        public virtual SubUnit SubUnit { get; set; }

        public int ? TeamId { get; set; }
        public virtual Team Team { get; set; }

        public int? LocationId { get; set; }
        public virtual Location Location { get; set; }








    }
}
