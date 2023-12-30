using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.AddNewSubUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.UpdateSubUnit;


namespace MakeItSimple.WebApi.Models.Setup.TeamSetup
{
    public partial class SubUnit
    {
        public class SubUnitValidator : AbstractValidator<AddNewSubUnitCommand>
        {
            public SubUnitValidator()
            {
                RuleFor(x => x.SubUnit_Code).NotEmpty().WithMessage("Sub unit code is required")
                    .MinimumLength(2).WithMessage("Sub unit code must be at least 2 character long!");

                RuleFor(x => x.SubUnit_Name).NotEmpty().WithMessage("Sub unit name is required")
                .MinimumLength(3).WithMessage("Sub unit name must be at least 3 character long!");
            }
        }

        public class UpdateSubUnitValidator : AbstractValidator<UpdateSubUnitCommand>
        {
            public UpdateSubUnitValidator()
            {
                RuleFor(x => x.SubUnit_Code).NotEmpty().WithMessage("Sub unit code is required")
                    .MinimumLength(2).WithMessage("Sub unit code must be at least 2 character long!");

                RuleFor(x => x.SubUnit_Name).NotEmpty().WithMessage("Sub unit name is required")
                .MinimumLength(3).WithMessage("Sub unit name must be at least 3 character long!");
            }
        }

    }

}
