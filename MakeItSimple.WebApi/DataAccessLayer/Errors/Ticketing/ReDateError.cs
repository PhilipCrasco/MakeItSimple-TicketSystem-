using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class ReDateError
    {
        public static Error TicketConcernIdNotExist() =>
        new Error("ReDateError.TicketConcernIdNotExist", "Ticket concern not exist!");

        public static Error TicketIdNotExist() =>
        new Error("ReDateError.TicketNotExist", "Ticket transaction not exist!");

        public static Error ApproverUnAuthorized() =>
        new Error("ReDateError.ApproverInvalid", "User not authorize to approve!");

        public static Error DateTimeInvalid() =>
        new Error("ReDateError.DateTimeInvalid", "Invalid start/target date!");

        public static Error ReDateIdNotExist() =>
        new Error("ReDateError.TicketNotExist", "Re-Date transaction not exist!");
    }
}
