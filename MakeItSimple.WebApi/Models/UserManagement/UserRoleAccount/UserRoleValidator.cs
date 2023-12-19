using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRole;

namespace MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount
{
    public partial class UserRole
    {
        public class UserRoleValidator : AbstractValidator<AddNewUserRoleCommand>
        {
            public UserRoleValidator()
            {

                RuleFor(x => x.Permissions).NotNull().NotEmpty();
                RuleFor(x => x.User_Role_Name).NotEmpty().WithMessage("User Role is required!")
               .MinimumLength(3).WithMessage("User Role must be at least 3 character long!");

            }
        }

        public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
        {
            public UpdateUserRoleValidator()
            {
                RuleFor(x => x.User_Role_Name).NotEmpty().WithMessage("User Role is required!")
               .MinimumLength(3).WithMessage("User Role must be at least 3 character long!");
            }
        }





    }
}
