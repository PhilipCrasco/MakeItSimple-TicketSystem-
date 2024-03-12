using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class ClosingTicketError
    {
      public static Error TicketConcernIdNotExist() =>
      new Error("ClosingtTicketError.TicketConcernIdNotExist", "Ticket concern not exist!");

      public static Error TicketIdNotExist() =>
      new Error("ClosingtTicketError.TicketNotExist", "Ticket transaction not exist!");

      public static Error ApproverUnAuthorized() =>
      new Error("ClosingtTicketError.ApproverInvalid", "User not authorize to approve!");

      public static Error DateTimeInvalid() =>
      new Error("ClosingtTicketError.DateTimeInvalid", "Invalid start/target date!");

      public static Error ReTicketConcernUnable() =>
      new Error("ClosingtTicketError.ClosingConcernUnable", "Re-Ticket request already in approval!");

      public static Error ClosingTicketConcernUnable() =>
      new Error("ClosingtTicketError.ClosingTicketConcernUnable", "Closing Ticket request already in approval!");

        public static Error TicketConcernIdAlreadyExist() =>
         new Error("ClosingtTicketError.TicketConcernIdAlreadyExist", "Ticket concern already exist!");

        public static Error ClosingTicketIdNotExist() =>
       new Error("TransferTicketError.ReTicketIdNotExist", "Closing Ticket request not exist!");

        public static Error ClosingTicketIdAlreadyExist() =>
       new Error("TransferTicketError.ClosingTicketIdAlreadyExist", "Closing Ticket request already exist!");

    }
}
