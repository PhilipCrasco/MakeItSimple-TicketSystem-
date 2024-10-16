using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport
{
    public partial class OpenTicketExport
    {
        public class OpenTicketExportCommand : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Unit { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }
    }
}
