namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewTicketHistory
{

    public partial class GetTicketHistory
    {
        public class GetTicketHistoryResult
        {
            public int? TicketConcernId { get; set; }

            public List<GetTicketHistoryConcern> GetTicketHistoryConcerns { get; set; }
            public class GetTicketHistoryConcern
            {
                public int TicketHistoryId { get; set; }
                public string Request { get; set; }
                public string Status { get; set; }
                public string Transacted_By { get; set; }
                public DateTime? Transaction_Date { get; set; }
                public string Remarks { get; set; }
                public int? Approver_Level { get; set; }
                public bool? IsApproved { get; set; }
            }

            public List<UpComingApprover> UpComingApprovers { get; set; }
            public class UpComingApprover
            {
                public int TicketHistoryId { get; set; }
                public string Request { get; set; }
                public string Status { get; set; }
                public string Transacted_By { get; set; }
                public DateTime? Transaction_Date { get; set; }
                public string Remarks { get; set; }
                public int? Approver_Level { get; set; }
                public bool? IsApproved { get; set; }
            }
        }

    }
}
