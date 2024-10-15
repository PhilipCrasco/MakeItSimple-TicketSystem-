using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup
{
    public class GetCategory
    {

        public class GetCategoryResult
        {
            public int Id { get; set; }
            public int ? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public string Category_Description { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }

            public string Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

            public bool Is_Active { get; set; }

            public List<Subcategory> subcategories { get; set; }

            public class Subcategory
            {
                public int SubCategoryId { get; set; }

                public string SubCategory_Description { get; set; }

            }


        }


        public class GetCategoryQuery : UserParams, IRequest<PagedList<GetCategoryResult>>
        {
            public string Search { get; set; }
            public bool ? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetCategoryQuery, PagedList<GetCategoryResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetCategoryResult>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Category> categoriesQuery = _context.Categories.Include(x => x.SubCategories)
                    .Include(x => x.Channel)
                    .Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if(!string.IsNullOrEmpty(request.Search))
                {
                    categoriesQuery = categoriesQuery.Where(x => x.CategoryDescription.Contains(request.Search));
                }

                if(request.Status != null)
                {
                    categoriesQuery = categoriesQuery.Where(x => x.IsActive == request.Status);
                }

                var results = categoriesQuery.Select(x => new GetCategoryResult
                {
                    Id = x.Id,
                    ChannelId = x.ChannelId,
                    Channel_Name = x.Channel.ChannelName,
                    Category_Description = x.CategoryDescription,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Is_Active = x.IsActive,
                    subcategories = x.SubCategories.Where(x => x.IsActive == true).Where(x => x.IsActive == true).Select(x => new GetCategoryResult.Subcategory
                    {
                        SubCategoryId = x.Id,
                        SubCategory_Description = x.SubCategoryDescription,

                    }).ToList()


                });

                return await PagedList<GetCategoryResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
