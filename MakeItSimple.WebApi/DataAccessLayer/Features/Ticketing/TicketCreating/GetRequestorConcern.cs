using MakeItSimple.WebApi.Models.Setup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating
{
    public class GetRequestorConcern
    {
        public class GetRequestConcernResult
        {
            public int ? RequestGeneratorId { get; set; }
            public Guid ? UserId { get; set; }  

            public string EmpId { get; set; }

            public string FullName { get; set; }

            public class RequestConcern
            {
                public int? RequestConcernId { get; set; }
                public string Concern_Status { get; set; }

                public bool Is_Done { get; set; }
                public string Remarks { get; set; }
            }

        }
    }
}
