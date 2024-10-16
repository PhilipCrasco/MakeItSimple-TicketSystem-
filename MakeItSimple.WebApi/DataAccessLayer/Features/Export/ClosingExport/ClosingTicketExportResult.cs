namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {
        public record class ClosingTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string Personnel { get; set; }
            public int? Ticket_Number { get; set; }
            public string Description { get; set; }
            public DateTime Target_Date { get; set; }
            public DateTime Actual { get; set; }
            public int Varience { get; set; }
            public decimal? Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }

        }
    }
}
