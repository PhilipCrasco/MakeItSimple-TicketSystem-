using MakeItSimple.WebApi.Common;


namespace MakeItSimple.WebApi.DataAccessLayer.Errors
{
    public class UserError
    {

        public static Error UserAlreadyExist(string EmpId, string Fullname) =>
        new ("User.UserAlreadyExist", $"User {EmpId} , {Fullname} already Exist");

        public static Error UsernameAlreadyExist(string Username) =>
        new ("User.UsernameAlreadyExist", $"Username {Username} already Exist");

        public static Error EmailAlreadyExist(string Email) =>
        new ("User.EmailAlreadyExist", $"Email {Email} already Exist");

        public static Error UserRoleNotExist() =>
        new ("User.UserRoleNotExist", "User role not exist!");

        public static Error DepartmentNotExist() =>
        new ("User.DepartmentNotExist", "Department not exist!");

        public static Error SubUnitNotExist() =>
        new ("User.SubUnitNotExist", "Sub unit not exist!");

        public static Error CompanyNotExist() =>
        new ("User.CompanyNotExist", "Company not exist!");
        public static Error LocationNotExist() =>
        new ("User.LocationNotExist", "Location not exist!");
        public static Error BusinessUnitNotExist() =>
        new ("User.BusinessUnitNotExist", "Business unit not exist!");

        public static Error ReceiverNotExist() =>
        new ("User.ReceiverNotExist", "Receiver must be set!");
        public static Error UnitNotExist() =>
        new ("User.UnitNotExist", "Unit not exist!");
        public static Error UserNotExist() =>
        new ("User.UserNotExist", "User not exist!");

        public static Error UserNoChanges() =>
        new ("User.UserNoChanges", "No changes has made!");

        public static Error UserIsUse(string Fullname) =>
        new ("User.UserIsUse", $"User {Fullname} was in use!"); 

        public static Error UserOldPasswordInCorrect() =>
        new ("User.UserNotExist", "Current password is incorrect!");

        public static Error UserPasswordShouldChange() =>
        new ("User.UserPasswordShouldChange", "User password must be change!");

        public static Error NewPasswordInvalid() =>
        new ("User.UserPasswordShouldChange", "New and confirm password not match!");

        public static Error InvalidDefaultPassword() =>
        new ("User.InvalidDefaultPassword", "New password should not be equal to default password!");

        public static Error ProfilePicNull() =>
        new ("User.ProfilePicNull", "Profile picture is empty!");    



    }
}