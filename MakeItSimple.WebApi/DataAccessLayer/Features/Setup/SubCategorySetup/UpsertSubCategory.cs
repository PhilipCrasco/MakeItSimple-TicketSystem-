using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup
{
    public class UpsertSubCategory
    {
        public class AddSubCategoryResult
        {
            public int Id { get; set; }
            public string SubCategory_Description { get; set; }
            public int CategoryId { get; set; }
            public Guid ? Added_By { get; set; }
            public DateTime Created_At { get; set; }
            
        }

        public class UpdateSubCategoryResult
        {
            public int Id { get; set; }
            public string SubCategory_Description { get; set; }
            public int CategoryId { get; set; }
            public Guid? Modified_By { get; set; }
            public DateTime ? Updated_At { get; set; }

        }


        public class UpsertSubCategoryCommand : IRequest<Result>
        {
            public int ? Id { get; set; }
            public string SubCategory_Description { get; set; }
            public int CategoryId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

        }

        public class Handler : IRequestHandler<UpsertSubCategoryCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertSubCategoryCommand command, CancellationToken cancellationToken)
            {

                var subCategoryAlreadyExist = await _context.SubCategories.FirstOrDefaultAsync(x => x.SubCategoryDescription == command.SubCategory_Description, cancellationToken);

                if (subCategoryAlreadyExist != null)
                {
                    return Result.Failure(SubCategoryError.SubCategoryAlreadyExist(command.SubCategory_Description));
                }

                var categoryNotExist = await _context.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId , cancellationToken);
                if(categoryNotExist == null)
                {
                    return Result.Failure(SubCategoryError.CategoryNotExist());
                }

                var subCategory = await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == command.Id , cancellationToken);

                if(subCategory != null)
                {
                    if(subCategory.SubCategoryDescription == command.SubCategory_Description && subCategory.CategoryId == command.CategoryId)
                    {
                        return Result.Failure(SubCategoryError.SubcategoryNochanges());
                    }

                    subCategory.SubCategoryDescription = command.SubCategory_Description;
                    subCategory.CategoryId = command.CategoryId;
                    subCategory.ModifiedBy = command.Modified_By;
                    subCategory.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync(cancellationToken);

                    var results = new UpdateSubCategoryResult
                    {
                        Id = subCategory.Id,
                        SubCategory_Description = subCategory.SubCategoryDescription,
                        CategoryId = subCategory.CategoryId,
                        Modified_By = subCategory.ModifiedBy,
                        Updated_At = subCategory.UpdatedAt,

                    };

                    return Result.Success(results);

                }
                else
                {

                    var addSubCategory = new SubCategory
                    {
                        SubCategoryDescription = command.SubCategory_Description,
                        CategoryId = command.CategoryId,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,
                    };

                    await _context.SubCategories.AddAsync(addSubCategory, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var results = new AddSubCategoryResult
                    {
                        Id = addSubCategory.Id,
                        SubCategory_Description = addSubCategory.SubCategoryDescription,
                        CategoryId = addSubCategory.CategoryId,
                        Added_By = addSubCategory.AddedBy,
                        Created_At = addSubCategory.CreatedAt,

                    };

                    return Result.Success(results);

                }



            }

        }



    }
}
