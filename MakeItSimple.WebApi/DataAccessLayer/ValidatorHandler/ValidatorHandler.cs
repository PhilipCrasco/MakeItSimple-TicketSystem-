using FluentValidation;
using MakeItSimple.WebApi.Models;
using static MakeItSimple.DataAccessLayer.Features.UserFeatures.AddNewUser;

namespace MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler
{
    public class ValidatorHandler
    {

        public  IValidator<AddNewUserCommand> AddNewUserValidator { get; set; }

        public ValidatorHandler()
        {
            AddNewUserValidator = new User.UserValidator();


        }
        
    }
}
