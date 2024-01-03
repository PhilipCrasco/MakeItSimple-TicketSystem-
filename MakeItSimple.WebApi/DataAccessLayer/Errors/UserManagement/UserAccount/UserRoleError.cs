using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount
{
    public class UserRoleError
    {

      public static Error UserRoleAlreadyExist( string UserRoleName) =>
      new Error("UserRole.UserRoleAlreadyExist", $"User Role  {UserRoleName} already Exist");

      public static Error UserRoleNotExist() =>
      new Error("UserRole.UserRoleNotExist", $"User Role not exist!");

      public static Error UserRoleIsUse(string UserRoleName) =>
      new Error("UserRole.UserRoleIsUse", $"User Role {UserRoleName} was use!");

      public static Error UserRoleNoChanges() =>
      new Error("UserRole.UserRoleNoChanges", "No changes has made!");

    }
}
