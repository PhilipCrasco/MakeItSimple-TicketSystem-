using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpsertCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup.UpsertSubCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.AddNewSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.UpdateSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.AddNewUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserFeatures.UpdateUser;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount.UserChangePassword;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.AddNewUserRole;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UntagAndTagUserRolePermission;
using static MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserRoleAccount.UpdateUserRole;
using static MakeItSimple.WebApi.Models.Setup.TeamSetup.SubUnit;
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


        // SubUnit Setup
        public IValidator<AddNewSubUnitCommand> AddNewSubUnitValidator { get; set; }
        public IValidator<UpdateSubUnitCommand> UpdateSubUnitValidator { get; set; }

        // Channel Setup 
        public IValidator<AddNewChannelCommand> AddNewChannelValidator {  get; set; }
        public IValidator<UpdateChannelCommand> UpdateChannelValidator {  get; set; }

        //Category Setup
        public IValidator<UpsertCategoryCommand> UpsertCategoryValidator { get; set; }

        //Sub Category Setup 

        public IValidator<UpsertSubCategoryCommand> UpsertSubCategoryValidator { get; set; }



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


            //SubUNit Setup

            AddNewSubUnitValidator = new SubUnit.SubUnitValidator();
            UpdateSubUnitValidator = new SubUnit.UpdateSubUnitValidator();

            //Channel Setup

            AddNewChannelValidator = new Channel.ChannelValidator();
            UpdateChannelValidator = new Channel.UpdateChannelValidator();

            //Category Setup
            UpsertCategoryValidator = new Category.CategoryValidator();

            //Sub Category Setup

            UpsertSubCategoryValidator = new SubCategory.SubCategoryValidator();

        }
        
    }
}
