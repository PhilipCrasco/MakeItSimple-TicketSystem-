using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class ClosingTicketError
    {
      public static Error TicketConcernIdNotExist() =>
      new Error("ClosintTicketError.TicketConcernIdNotExist", "Ticket concern not exist!");
    }
}
