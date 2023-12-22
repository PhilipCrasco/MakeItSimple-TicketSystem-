using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.GetCompany;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.AccountTitleSetup
{
    public class GetAccountTitle
    {
        public class GetAccountTitleResult
        {
            public int Id { get; set; }
            public int Account_No { get; set; }
            public string Account_Code { get; set; }
            public string Account_Titles { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }
        }

        public class GetAccountTitleQuery : UserParams, IRequest<PagedList<GetAccountTitleResult>>
        {
            public string Search { get; set; }
        }

        public  class Handler : IRequestHandler<GetAccountTitleQuery, PagedList<GetAccountTitleResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetAccountTitleResult>> Handle(GetAccountTitleQuery request, CancellationToken cancellationToken)
            {
                IQueryable<AccountTitle> accountTitlesQuery = _context.AccountTitles.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    accountTitlesQuery = accountTitlesQuery.Where(x => x.AccountCode.Contains(request.Search) || x.AccountTitles.Contains(request.Search));

                }

                var results = accountTitlesQuery.Select(x => new GetAccountTitleResult
                {
                    Id = x.Id,
                    Account_No = x.AccountNo,
                    Account_Code = x.AccountCode,
                    Account_Titles = x.AccountTitles,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    Sync_Status = x.SyncStatus,
                    SyncDate = x.SyncDate,

                });

                return await PagedList<GetAccountTitleResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
