using FluentValidation;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRole;



namespace MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler
{
    public class ValidatorHandler
    {
        //User
        public  IValidator<AddNewUserCommand> AddNewUserValidator { get; set; }
        public IValidator<UpdateUserCommand> UpdateUserValidator { get; set; }

        //UserRoles
        public IValidator<AddNewUserRoleCommand>  AddUserRoleValidator { get; set; }
        public IValidator<UpdateUserRoleCommand>  UpdateUserRoleValidator { get; set; }



        public ValidatorHandler()
        {

            //User

            AddNewUserValidator = new User.UserValidator();
            UpdateUserValidator = new User.UserUpdateValidator();

            //UserRole

            AddUserRoleValidator = new UserRole.UserRoleValidator();
            UpdateUserRoleValidator = new UserRole.UpdateUserRoleValidator();
        }
        
    }
}
