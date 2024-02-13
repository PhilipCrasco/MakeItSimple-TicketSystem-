using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UserChangePassword;


namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public class UserValidator : AbstractValidator<AddNewUserCommand>
        {
            public UserValidator()
            {

                RuleFor(f => f.Fullname).NotEmpty().WithMessage("Fullname is required!")
                .MinimumLength(3).WithMessage("Fullname must be at least 3 character long!");
                RuleFor(u => u.Username).NotEmpty().WithMessage("Username is required!")
                .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
               
            }

        }


        public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordCommand>
        {
            public UserChangePasswordValidator()
            {

                RuleFor(p => p.New_Password).Equal(p => p.Confirm_Password)
                    .WithMessage("New password not equal to confirm password!");

            }

        }






    }
}
