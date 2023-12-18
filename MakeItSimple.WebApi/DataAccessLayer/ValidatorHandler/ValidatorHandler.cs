using FluentValidation;
using MakeItSimple.WebApi.Models;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;



namespace MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler
{
    public class ValidatorHandler
    {

        public  IValidator<AddNewUserCommand> AddNewUserValidator { get; set; }
        public IValidator<UpdateUserCommand> UpdateUserValidator { get; set; }


        public ValidatorHandler()
        {
            AddNewUserValidator = new User.UserValidator();
            UpdateUserValidator = new User.UserUpdateValidator();
        }
        
    }
}
