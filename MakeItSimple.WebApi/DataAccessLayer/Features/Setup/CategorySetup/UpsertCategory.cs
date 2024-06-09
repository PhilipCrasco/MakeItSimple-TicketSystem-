
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup
{
    public class UpsertCategory
    {

        public class AddNewCategoryResult
        {
            public int Id { get; set; }
            public string Category_Description { get; set; }
            public Guid ? Added_By { get; set; }
            public DateTime Created_At { get; set; }

        }

        public class UpdateCategoryResult
        {
            public int Id { get; set; }
            public string Category_Description { get; set; }
            public Guid? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

        }


        public class UpsertCategoryCommand : IRequest<Result>
        {
            public int ? Id { get; set; }
            public string Category_Description { get; set; }
            public Guid? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }

        }

        public class Handler : IRequestHandler<UpsertCategoryCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertCategoryCommand command, CancellationToken cancellationToken)
            {
              
                var CategoryAlreadyExist = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryDescription == command.Category_Description , cancellationToken);

                if (CategoryAlreadyExist != null)
                {
                    return Result.Failure(CategoryError.CategoryAlreadyExist(command.Category_Description));
                }

                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                
                if (category != null )
                {
                    if(category.CategoryDescription  == command.Category_Description)
                    {
                        return Result.Failure(CategoryError.CategoryNochanges());
                    }
                    
                    var categoryIsUse = await _context.SubCategories.AnyAsync(x => x.CategoryId == command.Id && x.IsActive == true, cancellationToken);
                     
                    if(categoryIsUse == true )
                    {
                        return Result.Failure(CategoryError.CategoryIsUse(category.CategoryDescription));
                    }

                    category.CategoryDescription = command.Category_Description;
                    category.ModifiedBy = command.Modified_By;
                    category.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync(cancellationToken);

                    var results = new UpdateCategoryResult
                    {
                        Id = category.Id,
                        Category_Description = category.CategoryDescription,
                        Modified_By = category.ModifiedBy,
                        Updated_At = category.UpdatedAt
                    };

                    return Result.Success(results);
                }
                else
                {
                    var addCategory = new Category
                    {
                        CategoryDescription = command.Category_Description,
                        AddedBy = command.Added_By,

                    };

                    await _context.Categories.AddAsync(addCategory, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);

                    var results = new AddNewCategoryResult
                    {
                        Id = addCategory.Id,
                        Category_Description = addCategory.CategoryDescription,
                        Added_By = addCategory.AddedBy,
                        Created_At = addCategory.CreatedAt
                    };

                    return Result.Success(results);
                }
     
            }
        }





    }
}
