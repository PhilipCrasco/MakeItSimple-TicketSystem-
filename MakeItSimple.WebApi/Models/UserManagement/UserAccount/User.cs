
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
        public string Email { get; set; }
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


    }
}
