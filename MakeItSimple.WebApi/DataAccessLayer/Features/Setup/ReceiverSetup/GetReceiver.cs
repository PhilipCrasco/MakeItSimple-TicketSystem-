using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup.GetReceiver.GetReceiverResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup
{
    public class GetReceiver
    {
        public class GetReceiverResult
        {
            public Guid ? UserId { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public DateTime? Updated_At { get; set; }
            public virtual string Modified_By { get; set; }
            public bool Is_Active { get; set; }

            public List<GetReceive> GetReceives { get; set; }

            public class GetReceive
            {
                public int ReceiveId { get; set; }

                public int ? BusinessUnitId { get; set; }
                public string BusinessUnit_Code { get; set; }
                public string BusinessUnit_Name { get; set; }

            }

        }


        public class GetReceiverQuery : UserParams , IRequest<PagedList<GetReceiverResult>>
        {
            public string Search { get; set; }
            public bool ? Status {  get; set; }
        }

        public class Handler : IRequestHandler<GetReceiverQuery, PagedList<GetReceiverResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetReceiverResult>> Handle(GetReceiverQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Receiver> receiverQuery = _context.Receivers.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    receiverQuery = receiverQuery.Where(x => x.User.Fullname.Contains(request.Search) || x.User.EmpId.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    receiverQuery = receiverQuery.Where(x => x.IsActive == request.Status);
                }

                var results = receiverQuery.GroupBy(x => x.UserId).Select(x => new GetReceiverResult
                {

                    UserId = x.Key,
                    FullName = x.First().User.Fullname, 
                    UserName = x.First().User.Username,
                    Created_At = x.First().CreatedAt,
                    Added_By = x.First().AddedByUser.Fullname,
                    Updated_At = x.First().UpdatedAt,
                    Is_Active = x.First().IsActive,
                    GetReceives = x.Select(x => new GetReceiverResult.GetReceive
                    {
                        ReceiveId  = x.Id,
                        BusinessUnitId = x.BusinessUnitId,
                        BusinessUnit_Code = x.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = x.BusinessUnit.BusinessName


                    }).ToList()


                });



                return await PagedList<GetReceiverResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
