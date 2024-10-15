
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup
{
    public class UpsertCategory
    {


        public class UpsertCategoryCommand : IRequest<Result>
        {
            public int ? Id { get; set; }
            public int? ChannelId { get; set; }
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

                var channelExist = await _context.Channels
                    .FirstOrDefaultAsync(x => x.Id == command.ChannelId , cancellationToken);

                if (channelExist is null)
                    return Result.Failure(TicketRequestError.ChannelNotExist());


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
                    category.ChannelId = command.ChannelId;

                    await _context.SaveChangesAsync(cancellationToken);

                    return Result.Success();
                }
                else
                {
                    var addCategory = new Category
                    {
                        ChannelId = command.ChannelId,
                        CategoryDescription = command.Category_Description,
                        AddedBy = command.Added_By,


                    };

                    await _context.Categories.AddAsync(addCategory, cancellationToken);

                    await _context.SaveChangesAsync(cancellationToken);


                    return Result.Success();
                }
     
            }
        }





    }
}
