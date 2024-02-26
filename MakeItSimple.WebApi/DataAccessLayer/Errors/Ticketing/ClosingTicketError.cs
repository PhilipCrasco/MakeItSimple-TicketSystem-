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

    }
}
