using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup
{
    public class UpdateCategoryStatus
    {
        public class UpdateCategoryStatusResult
        {
            public int Id { get; set; }
            public bool Status {  get; set; }
        }

        public class UpdateCategoryStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateCategoryStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateCategoryStatusCommand command, CancellationToken cancellationToken)
            {

                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id  == command.Id , cancellationToken);

                if (category == null)
                {
                    return Result.Failure(CategoryError.CategoryNotExist());
                }

                var categoryIsUse = await _context.SubCategories.AnyAsync(x => x.CategoryId == command.Id && x.IsActive == true, cancellationToken);

                if (categoryIsUse == true)
                {
                    return Result.Failure(CategoryError.CategoryIsUse(category.CategoryDescription));
                }

                category.IsActive = !category.IsActive;
                
                await _context.SaveChangesAsync(cancellationToken);

                var results = new UpdateCategoryStatusResult
                {
                    Id = category.Id,
                    Status = category.IsActive

                };

                return Result.Success(results);

            }



        }
    }
}
