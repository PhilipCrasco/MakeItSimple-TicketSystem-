using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer
{
    public partial class GetTransferTicket
    {
        public class GetTransferTicketQuery : UserParams, IRequest<PagedList<GetTransferTicketResult>>
        {
            public Guid? UserId { get; set; }
            //public string Approval { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public bool? IsTransfer { get; set; }
            public bool? IsReject { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }



        }
    }
}
