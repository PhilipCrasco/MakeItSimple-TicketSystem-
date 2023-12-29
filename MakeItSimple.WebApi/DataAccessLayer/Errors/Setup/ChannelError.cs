using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class ChannelError
    {

      public static Error ChannelNameAlreadyExist(string ChannelName) =>
      new Error("Channel.ChannelNameAlreadyExist", $"Channel name {ChannelName} already exist!");

      public static Error SubUnitNotExist() =>
      new Error("Channel.SubUnitNotExist", "Sub unit doesn't exist!");

      public static Error UserNotExist() =>
      new Error("Channel.UserNotExist", "User doesn't exist!");

      public static Error ChannelNotExist() =>
      new Error("Channel.ChannelNotExist", "Channel doesn't exist!");

      public static Error ChannelUserNotExist() =>
      new Error("Channel.ChannelUserNotExist", "Member doesn't exist!");

      public static Error UserAlreadyAdd() =>
      new Error("Channel.UserAlreadyAdd", "Member already been add!");

    }
}
