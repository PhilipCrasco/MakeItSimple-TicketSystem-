using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors
{
    public class AuthenticationError
    {

      public static Error UsernameAndPasswordIncorrect() =>
      new Error("Authentication.UsernameAndPasswordIncorrect", "Username or password is incorrect!");
      public static Error UserNotActive() =>
      new Error("Authentication.UserNotActive", "User not active!");
      public static Error NoRole() =>
      new Error("Authentication.UsernameAndPasswordIncorrect", "No role assigned to this user!");

    }
}
