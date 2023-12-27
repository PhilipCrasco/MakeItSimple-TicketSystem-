using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class TeamError
    {
      public static Error TeamAlreadyExist() =>
      new Error("Team.TeamAlreadyExist", "Team already exist!");

    }
}
