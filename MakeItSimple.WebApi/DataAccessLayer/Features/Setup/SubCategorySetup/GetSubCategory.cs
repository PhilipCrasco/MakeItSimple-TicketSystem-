using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup
{
    public class GetSubCategory
    {

        public class GetSubCategoryResult
        {
            public int Id { get; set; }
            public string SubCategory_Description { get; set; }
            public int ? CategoryId {  get; set; }
            public string Category_Description { get; set; }
            public bool Is_Active { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

        }

        public class GetSubCategoryQuery : UserParams, IRequest<PagedList<GetSubCategoryResult>>
        {
            public string Search { get; set; }
            public bool ? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetSubCategoryQuery, PagedList<GetSubCategoryResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetSubCategoryResult>> Handle(GetSubCategoryQuery request, CancellationToken cancellationToken)
            {
                IQueryable<SubCategory> subCategoriesQuery = _context.SubCategories.Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser).Include(x => x.Category);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    subCategoriesQuery = subCategoriesQuery.Where(x => x.SubCategoryDescription.Contains(request.Search) 
                    || x.Category.CategoryDescription.Contains(request.Search));
                }

                if(request.Status != null) 
                {
                   subCategoriesQuery = subCategoriesQuery.Where(x => x.IsActive == request.Status);
                }

                var results = subCategoriesQuery.Select(x => new GetSubCategoryResult
                {
                    Id = x.Id,
                    SubCategory_Description = x.SubCategoryDescription,
                    CategoryId = x.CategoryId,
                    Category_Description = x.Category.CategoryDescription,
                    Is_Active = x.IsActive,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt

                });

                return await PagedList<GetSubCategoryResult>.CreateAsync(results, request.PageNumber , request.PageSize);

            }

        }
    }
}
