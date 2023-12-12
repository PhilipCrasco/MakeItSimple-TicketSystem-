using FluentValidation;
using MakeItSimple.Utility.Common;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.Domain.Models.UserModel
{
    public class User 
    {
        public Guid Id { get; set; }
        public bool IsActive { get ; set; } = true;
        [Required]
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;}
     

        

        public class UserValidator : AbstractValidator<User>
        {
            public UserValidator()
            {
                RuleFor(x => x.Id).NotNull();
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required!")
                    .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!")
                    .MinimumLength(6).WithMessage("Password must be at least 6 character/number long!");
            }
        }



    }
}
