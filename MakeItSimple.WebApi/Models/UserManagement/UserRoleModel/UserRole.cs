using FluentValidation;
using MakeItSimple.WebApi.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace MakeItSimple.WebApi.Models.UserManagement.UserRoleModel
{
    public class UserRole : BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AddedByUser")]
        public Guid ? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public string UserRoleName { get; set; }
        public ICollection<string> Permissions { get; set; }
        public ICollection<User> Users { get; set; }


        public class UserRoleValidator : AbstractValidator<UserRole>
        {
            public UserRoleValidator()
            {
                RuleFor(x => x.Id).NotNull();
                RuleFor(x => x.Permissions).NotNull().NotEmpty();
                RuleFor(x => x.UserRoleName).NotEmpty().WithMessage("User Role is required!")
               .MinimumLength(3).WithMessage("User Role must be at least 3 character long!");
                RuleFor(x => x.AddedBy).NotNull();
                RuleFor(x => x.ModifiedBy).NotNull();
            }
        }



    }
}
