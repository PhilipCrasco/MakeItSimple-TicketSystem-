using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class ReTicketConcernError
    {

      public static Error TicketIdNotExist() =>
      new Error("ReTicketConcern.TicketNotExist", "Ticket transaction not exist!");

      public static Error TicketConcernIdNotExist() =>
      new Error("ReTicketConcern.TicketConcernIdNotExist", "Ticket concern not exist!");

      public static Error TicketConcernIdAlreadyExist() =>
      new Error("ReTicketConcern.TicketConcernIdAlreadyExist", "Ticket concern already exist!");

      public static Error ReTicketConcernUnable() =>
      new Error("ReTicketConcern.ReTicketConcernUnable", "Re-Ticket request already in approval!");

      public static Error ReTicketIdAlreadyExist() =>
      new Error("TransferTicketError.ReTicketIdAlreadyExist", "Re-Ticket request already exist!");
       public static Error ReTicketIdNotExist() =>
      new Error("TransferTicketError.ReTicketIdNotExist", "Re-Ticket request not exist!");
        public static Error DateTimeInvalid() =>
      new Error("ReTicketConcern.DateTimeInvalid", "Invalid start/target date!");

      public static Error ApproverUnAuthorized() =>
      new Error("ReTicketConcern.ApproverInvalid", "User not authorize to approve!");

    }
}
