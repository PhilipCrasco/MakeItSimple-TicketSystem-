using System.Reflection.Metadata;

namespace MakeItSimple.WebApi.Common.ConstantString
{
    public class TicketingConString
    {

        public const string Approval = "For Approval";
        public const string OnGoing = "Ongoing";
        public const string Done = "Done";



        public const string Users = "Users";
        public const string ApproverTransfer = "Admin";

        // Ticketing Role

        public const string Admin = "Admin";

        public const string Approver = "Approver";
        public const string Requestor = "Requestor";
        public const string Receiver = "Receiver";
        public const string IssueHandler = "Issue Handler";
        public const string Support = "Support";

        //TicketHistory

        public const string RequestCreated = "A concern was created by ";
        public const string RequestAssign = "A concern was assigned to ";

        public const string CloseRequest = "Ticket closure requested";
        public const string CloseCancel = "Ticket closure request canceled";
        public const string CloseReject = "Ticket closure request rejected by";
        public const string CloseApprove = "Ticket closure request approved by";
        public const string CloseApproveReceiver = "Receiver approved the ticket closure request";
        public const string CloseConfirm = "Ticket confirmed as resolved";
        public const string CloseReturn = "Ticket remains open due to unresolved issues";



        public const string RejectedBy = "Request was rejected";
        public const string RequestUpdate = "Request Updated";
        public const string ApproveBy = "Request approve by";
        public const string ReceiverApproveBy = "Request has been approved by Receiver";
        public const string Returned = "Request was returned";
        public const string Cancel = "Request was cancel";


        // Concern Status 

        public const string ForApprovalTicket = "For Approval";
        public const string CurrentlyFixing = "Ongoing";
        public const string ConcernStatus = "ConcernStatus";

        public const string Concern = "Concern";
        public const string Manual = "Manual";
        public const string RequestTicket = "Request Ticket";  

        public const string ForReticket = "For Re-Ticket";
        public const string ForClosing = "For Closing Ticket";
        public const string ForTransfer = "For Transfer Ticket";
        public const string OpenTicket = "Open Ticket";
        public const string PendingRequest = "Pending Request";
        public const string ForReDate = "For Re-Date";

        public const string ClosedTicket = "ClosedTicket";
        public const string TransferTicket = "Transfer Ticket";
        public const string CloseTicket = "Closing Ticket";

        public const string Open = "Open Ticket";

        public const string ReTicket = "Re-Ticket";
        public const string ReDate = "Re-Date";
        public const string Closed = "Closed";
        public const string Transfer = "Transfer";
        public const string Request = "Request Concern";
        public const string NotConfirm = "Closed/For Confirmation";



        public const string TicketClosed = "Ticket Closed";

        public const string RequestApproval = "For approval by ";


    }
}
