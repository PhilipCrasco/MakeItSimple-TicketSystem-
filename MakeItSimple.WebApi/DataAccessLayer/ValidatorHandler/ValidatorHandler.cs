using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.AddNewTeam;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UserChangePassword;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UntagAndTagUserRolePermission;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRole;
using static MakeItSimple.WebApi.Models.User;



namespace MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler
{
    public class ValidatorHandler
    {
        //User
        public  IValidator<AddNewUserCommand> AddNewUserValidator { get; set; }
        public IValidator<UpdateUserCommand> UpdateUserValidator { get; set; }
        public IValidator<UserChangePasswordCommand> UserChangePasswordValidator { get; set; }


        //UserRoles
        public IValidator<AddNewUserRoleCommand>  AddUserRoleValidator { get; set; }
        public IValidator<UpdateUserRoleCommand>  UpdateUserRoleValidator { get; set; }
        public IValidator<UntagAndTagUserRolePermissionCommand> TagAndUntagUserRoleValidator {  get; set; }


        // Team Setup
        public IValidator<AddNewTeamCommand> AddNewTeamValidator { get; set; }



        public ValidatorHandler()
        {

            //User

            AddNewUserValidator = new User.UserValidator();
            UpdateUserValidator = new User.UserUpdateValidator();
            UserChangePasswordValidator = new User.UserChangePasswordValidator();   

            //UserRole

            AddUserRoleValidator = new UserRole.UserRoleValidator();
            UpdateUserRoleValidator = new UserRole.UpdateUserRoleValidator();
            TagAndUntagUserRoleValidator = new UserRole.TagAndUntagUserRoleValidator();


            //Team Setup

            AddNewTeamValidator = new Team.TeamValidator();
        }
        
    }
}
