using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors
{
    public class AuthenticationError
    {

      public static Error UsernameAndPasswordIncorrect() =>
      new Error("Authentication.UsernameAndPasswordIncorrect", "Username/Email or password is incorrect!");

      public static Error NoRole() =>
      new Error("Authentication.UsernameAndPasswordIncorrect", "No role assigned to this user!");

    }
}
