namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport
{
    public partial class OnHoldTicketReport
    {
        public record OnHoldTicketReportResult 
        {
            public int TicketConcernId { get; set; }
            public string Concerns {  get; set; }
            public string Reason { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool? IsHold { get; set; }
            public DateTime? Resume_At { get; set; }
            

        }
    }
}
