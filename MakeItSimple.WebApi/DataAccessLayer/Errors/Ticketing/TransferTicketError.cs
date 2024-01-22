using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TransferTicketError
    {
      public static Error TicketConcernIdNotExist() =>
      new Error("TransferTicketError.TicketConcernIdNotExist", "Ticket concern not exist!");

       public static Error TransferTicketConcernIdNotExist() =>
      new Error("TransferTicketError.TransferTicketConcernIdNotExist", "Transfer ticket concern not exist!");

      public static Error SubUnitNotExist() =>
      new Error("TransferTicketError.SubUnitNotExist", "Sub unit not exist!");

      public static Error ChannelNotExist() =>
      new Error("TransferTicketError.ChannelNotExist", "Channel not exist!");
      
      public static Error TransferTicketAlreadyExist() =>
      new Error("TransferTicketError.TransferTicketAlreadyExist", "Transfer ticket already exist!");

      public static Error UserNotExist() =>
      new Error("TransferTicketError.UserNotExist", "Member not exist!");
      public static Error DuplicateTransferTicket() =>
      new Error("DuplicateTransferTicket.DuplicateTransferTicket", "Concern ticket duplicated!");
    }
}
