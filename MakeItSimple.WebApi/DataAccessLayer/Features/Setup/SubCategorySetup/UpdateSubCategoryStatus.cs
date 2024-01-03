using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup
{
    public class UpdateSubCategoryStatus
    {
        public class UpdateSubCategoryStatusResult
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }

        public class UpdateSubCategoryStatusCommand : IRequest<Result>
        {
            public int Id { get; set; } 
        }

        public class Handler : IRequestHandler<UpdateSubCategoryStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateSubCategoryStatusCommand command, CancellationToken cancellationToken)
            {
                var subCategory = await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (subCategory == null)
                {
                    return Result.Failure(SubCategoryError.SubCategoryNotExist());
                }

                subCategory.IsActive = !subCategory.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new UpdateSubCategoryStatusResult
                {
                    Id = subCategory.Id,
                    Status = subCategory.IsActive
                };

                return Result.Success(results);

            }
        }
    }
}
