using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing
{
    public class TicketRequestError
    {
      public static Error TicketIdNotExist() =>
      new Error("TicketRequest.TicketNotExist", "Ticket not exist!");
      public static Error DepartmentNotExist() =>
      new Error("TicketRequest.DepartmentNotExist", "Department not exist!");
      
      public static Error ConcernDetailsNotNull() =>
      new Error("TicketRequest.ConcernDetailsNotNull", "Concern details must not be empty!");
      
      public static Error SubUnitNotExist() =>
      new Error("TicketRequest.SubUnitNotExist", "Sub unit not exist!");

      public static Error ChannelNotExist() =>
      new Error("TicketRequest.ChannelNotExist", "Channel not exist!");

      public static Error UserNotExist() =>
      new Error("TicketRequest.UserNotExist", "Member not exist!");

      public static Error CategoryNotExist() =>
      new Error("TicketRequest.CategoryNotExist", "Category not exist!");
      
      public static Error SubCategoryNotExist() =>
      new Error("TicketRequest.SubCategoryNotExist", "Sub category not exist!");

      public static Error DateTimeInvalid() =>
      new Error("TicketRequest.DateTimeInvalid", "Invalid start/target date!");

      public static Error DuplicateConcern() =>
      new Error("TicketRequest.DateTimeInvalid", "Concern ticket duplicated!");

      public static Error AttachmentNotNull() =>
      new Error("TicketRequest.AttachmentNotNull", "Attachment must not be empty!");
     
      public static Error AttachmentAlreadyExist() =>
      new Error("TicketRequest.AttachmentNotNull", "Attachment already exist!");

      public static Error InvalidAttachmentSize() =>
      new Error("TicketRequest.InvalidAttachmentSize", "Invalid Attachment Size!");
      
      public static Error InvalidAttachmentType() =>
      new Error("TicketRequest.InvalidAttachmentType", "Invalid Attachment Type!");

    }
}
