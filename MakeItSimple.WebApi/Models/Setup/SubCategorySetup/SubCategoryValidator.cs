using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubCategorySetup.UpsertSubCategory;

namespace MakeItSimple.WebApi.Models.Setup.SubCategorySetup
{
    public partial class SubCategory
    {
        public class SubCategoryValidator : AbstractValidator<UpsertSubCategoryCommand>
        {
            public SubCategoryValidator()
            {
                RuleFor(x => x.SubCategory_Description).NotEmpty().WithMessage("Sub category description code is required")
                    .MinimumLength(3).WithMessage("Sub category description must be at least 3 character long!");
            }
        }
    }
}
