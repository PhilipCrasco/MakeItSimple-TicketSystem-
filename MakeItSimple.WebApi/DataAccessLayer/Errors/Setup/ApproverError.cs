using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup.SyncLocation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup.SyncLocation.SyncLocationCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class ApproverError
    {
       public static Error ChannelNotExist() =>
       new Error("Approver.ChannelNotExist", "Channel not exist!");

       public static Error ChannelAlreadyExist() =>
       new Error("Approver.ChannelAlreadyExist", "Channel already exist!");

       public static Error UserNotExist() =>
       new Error("Approver.UserNotExist", "Approver not exist!");

       public static Error UserAlreadyExist() =>
       new Error("Approver.UserAlreadyExist", "Approver already exist!");

       public static Error UserDuplicate() =>
       new Error("Approver.UserDuplicate", "Approver duplicated!");

       public static Error ApproverLevelDuplicate() =>
       new Error("Approver.ApproverLevelDuplicate", "Approver level duplicated!");

       public static Error UserNotAuthorize() =>
       new Error("Approver.UserDuplicate", "Approver not Authorized!");
       public static Error ApproverNotExist() =>
       new Error("Approver.ApproverNotExist", "Approver not Exist!");

       // public static Error ListError(List<SyncLocationCommand> AvailableSync ) =>
       //new Error("Approver.ApproverNotExist", $"{AvailableSync}");

    }
}
