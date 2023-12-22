using MakeItSimple.WebApi.Common;


namespace MakeItSimple.WebApi.DataAccessLayer.Errors
{
    public class UserError
    {

      public static Error UserAlreadyExist(string EmpId, string Fullname) =>
      new Error("User.UserAlreadyExist", $"User {EmpId} , {Fullname} already Exist");

      public static Error UsernameAlreadyExist(string Username) =>
      new Error("User.UsernameAlreadyExist", $"Username {Username} already Exist");

      public static Error EmailAlreadyExist(string Email) =>
      new Error("User.EmailAlreadyExist", $"Email {Email} already Exist");

      public static Error UserRoleNotExist() =>
      new Error("User.UserRoleNotExist", "User role not exist!");

      public static Error DepartmentNotExist() =>
      new Error("User.DepartmentNotExist", "Department not exist!");
      
      public static Error UserNotExist() =>
      new Error("User.UserNotExist", "User not exist!");

      public static Error UserOldPasswordInCorrect() =>
      new Error("User.UserNotExist", "Current password is incorrect!");

      public static Error UserPasswordShouldChange() =>
      new Error("User.UserPasswordShouldChange", "User password must be change!");

      public static Error InvalidDefaultPassword() =>
      new Error("User.InvalidDefaultPassword", "New password should not be equal to default password!");

     

    }
}
