using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class ReTicketConcernError
    {
      public static Error TicketConcernIdNotExist() =>
      new Error("ReTicketConcern.TicketConcernIdNotExist", "Ticket concern not exist!");

      public static Error TicketConcernIdAlreadyExist() =>
      new Error("ReTicketConcern.TicketConcernIdAlreadyExist", "Ticket concern already exist!");

        public static Error DateTimeInvalid() =>
        new Error("ReTicketConcern.DateTimeInvalid", "Invalid start/target date!");

    }
}
