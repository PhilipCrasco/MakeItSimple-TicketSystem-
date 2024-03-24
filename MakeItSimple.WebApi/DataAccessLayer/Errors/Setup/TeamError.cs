using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class TeamError
    {

        public static Error SubUnitNotExist() =>
        new Error("Team.SubUnitNotExist", "Sub unit doesn't exist!");

        public static Error TeamNameAlreadyExist() =>
        new Error("Team.TeamNameAlreadyExist", "Team name already exist!");
        public static Error TeamNotExist() =>
        new Error("Team.TeamNotExist", "Team doesn't exist!");
    }
}
