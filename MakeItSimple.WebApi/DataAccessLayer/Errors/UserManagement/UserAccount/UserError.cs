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

        public static Error SubUnitNotExist() =>
        new Error("User.SubUnitNotExist", "Sub unit not exist!");

        public static Error CompanyNotExist() =>
        new Error("User.CompanyNotExist", "Company not exist!");
        public static Error LocationNotExist() =>
        new Error("User.LocationNotExist", "Location not exist!");
        public static Error BusinessUnitNotExist() =>
        new Error("User.BusinessUnitNotExist", "Business unit not exist!");

        public static Error ReceiverNotExist() =>
        new Error("User.ReceiverNotExist", "Receiver must be set!");
        public static Error UnitNotExist() =>
        new Error("User.UnitNotExist", "Unit not exist!");
        public static Error UserNotExist() =>
        new Error("User.UserNotExist", "User not exist!");

        public static Error UserNoChanges() =>
        new Error("User.UserNoChanges", "No changes has made!");

        public static Error UserIsUse(string Fullname) =>
        new Error("User.UserIsUse", $"User {Fullname} was use!");

        public static Error UserOldPasswordInCorrect() =>
        new Error("User.UserNotExist", "Current password is incorrect!");

        public static Error UserPasswordShouldChange() =>
        new Error("User.UserPasswordShouldChange", "User password must be change!");

        public static Error NewPasswordInvalid() =>
        new Error("User.UserPasswordShouldChange", "New and confirm password not match!");

        public static Error InvalidDefaultPassword() =>
        new Error("User.InvalidDefaultPassword", "New password should not be equal to default password!");

        public static Error ProfilePicNull() =>
        new Error("User.ProfilePicNull", "Profile picture is empty!");    



    }
}