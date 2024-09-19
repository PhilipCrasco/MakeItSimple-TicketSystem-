using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.FormSetup
{
    public class GetForm
    {
        public class GetFormResult
        {
            public int Id { get; set; }
            public string Form_Name { get; set; }

            public string Added_By { get; set; }

            public DateTime Created_At  { get; set; }

            public string Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }
            public bool IsActive { get; set; }

        }

        public class GetFormQuery : UserParams,IRequest<PagedList<GetFormResult>>
        {
            public string Search {  get; set; }

            public bool ? Status { get; set; }

        }

        public class Handler : IRequestHandler<GetFormQuery, PagedList<GetFormResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetFormResult>> Handle(GetFormQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Form> formsQuery =  _context.Forms
                    .AsNoTracking()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser);


                if(!string.IsNullOrEmpty(request.Search))
                {
                    formsQuery = formsQuery
                        .Where(f => f.Form_Name.Contains(request.Search));
                }

                if(request.Status is not null)
                {
                    formsQuery = formsQuery
                        .Where(f => f.IsActive == request.Status);
                }

                var formResult = formsQuery
                    .Select(f => new GetFormResult
                    {
                        Id = f.Id,
                        Form_Name = f.Form_Name,
                        Added_By = f.AddedByUser.Fullname,
                        Created_At = f.CreatedAt,
                        Modified_By = f.ModifiedByUser.Fullname,
                        Updated_At = f.UpdatedAt,
                        IsActive = f.IsActive,

                    });



                return await PagedList<GetFormResult>.CreateAsync(formResult, request.PageNumber , request.PageSize);
            }
        }
    }
}
