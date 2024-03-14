using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class ReceiverError
    {
      public static Error BusinessUnitNotExist() =>
      new Error("Receiver.BusinessUnitNotExist", "Business unit doesn't exist!");

      public static Error UserUnitNotExist() =>
      new Error("Receiver.UserUnitNotExist", "User unit doesn't exist!");


        public static Error ReceiverNotExist() =>
        new Error("Receiver.ReceiverNotExist", "Receiver doesn't exist!");
        public static Error DuplicateReceiver() =>
      new Error("Receiver.UserUnitNotExist", "Business unit already exist!");

    }
}
