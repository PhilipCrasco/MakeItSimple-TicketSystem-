using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup
{
    public class GetCompany
    {

        public class GetCompanyResult
        {
            public int Id { get; set; }
            public int Company_No { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }
        }

        public class GetCompanyQuery : UserParams , IRequest<PagedList<GetCompanyResult>>
        {
            public string Search {  get; set; }

            public bool ? Status {  get; set; }
        }

        public class Handler : IRequestHandler<GetCompanyQuery, PagedList<GetCompanyResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetCompanyResult>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Company> companiesQuery = _context.Companies.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    companiesQuery = companiesQuery.Where(x => x.CompanyCode.Contains(request.Search) || x.CompanyName.Contains(request.Search));

                }

                if(request.Status != null)
                {
                    companiesQuery = companiesQuery.Where(x => x.IsActive == request.Status);
                }

                var results = companiesQuery.Select(x => new GetCompanyResult
                {
                    Id = x.Id,
                    Company_No = x.CompanyNo,
                    Company_Code = x.CompanyCode,
                    Company_Name = x.CompanyName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    Sync_Status = x.SyncStatus,
                    SyncDate = x.SyncDate,


                });


                return await PagedList<GetCompanyResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
