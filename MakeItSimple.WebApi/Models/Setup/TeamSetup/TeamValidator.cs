using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup.AddNewTeam;


namespace MakeItSimple.WebApi.Models.Setup.TeamSetup
{
    public partial class Team
    {
        public class TeamValidator : AbstractValidator<AddNewTeamCommand>
        {
            public TeamValidator()
            {
                RuleFor(tc => tc.Team_Code).NotEmpty().WithMessage("Team code is required")
                    .MinimumLength(2).WithMessage("Team code must be at least 2 character long!");

                RuleFor(tn => tn.Team_Name).NotEmpty().WithMessage("Team name is required")
                .MinimumLength(3).WithMessage("Team name must be at least 3 character long!");
            }
        }

    }

}
