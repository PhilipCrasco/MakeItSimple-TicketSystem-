using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CategorySetup.UpsertCategory;

namespace MakeItSimple.WebApi.Models.Setup.CategorySetup
{
    public partial class Category
    {
        public class CategoryValidator : AbstractValidator<UpsertCategoryCommand>
        {
            public CategoryValidator()
            {
                RuleFor(x => x.Category_Description).NotEmpty().WithMessage("Category description code is required")
                    .MinimumLength(3).WithMessage("Category description must be at least 3 character long!");
            }
        }

    }
}
