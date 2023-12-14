using FluentValidation;
using static MakeItSimple.DataAccessLayer.Features.UserFeatures.AddNewUser;

namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public class UserValidator : AbstractValidator<AddNewUserCommand>
        {
            public UserValidator()
            {
                RuleFor(x => x.Id).NotNull();
                RuleFor(x => x.Fullname).NotEmpty().WithMessage("Fullname is required!")
                .MinimumLength(3).WithMessage("Fullname must be at least 3 character long!");
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required!")
                .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!")
                .MinimumLength(6).WithMessage("Password must be at least 6 character/number long!");
                RuleFor(em => em.Email).NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("A valid email address is required!");
                    
              
            }
        }



    }
}
