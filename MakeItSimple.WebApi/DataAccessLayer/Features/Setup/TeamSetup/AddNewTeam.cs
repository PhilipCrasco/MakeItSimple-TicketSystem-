using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class AddNewTeam
    {

        public class AddNewTeamResult
        {
            public int Id { get; set; }
            public string Team_Code { get; set; }
            public string Team_Name { get; set; }
            public string Added_By { get; set; }

            public DateTime Created_At { get; set; }
        }


        public class AddNewTeamCommand : IRequest<Result>
        {
            public string Team_Code { get; set; }
            public string Team_Name { get; set; }
            public Guid? Added_By { get; set; }
        }


        public class Handler : IRequestHandler<AddNewTeamCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewTeamCommand command, CancellationToken cancellationToken)
            {

                var TeamAlreadyExist = await _context.Teams.FirstOrDefaultAsync(x => x.TeamCode == command.Team_Code 
                || x.TeamName == command.Team_Name ,cancellationToken);

                if(TeamAlreadyExist != null)
                {
                    return Result.Failure(TeamError.TeamAlreadyExist());
                }

                var team = new Team
                {
                    TeamCode = command.Team_Code,
                    TeamName = command.Team_Name,
                    AddedBy = command.Added_By,
                };

                await _context.Teams.AddAsync(team , cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var addedby = await _context.Users.FirstOrDefaultAsync(x => x.Id == team.AddedBy , cancellationToken);

                var results = new AddNewTeamResult
                {
                    Id = team.Id,
                    Team_Code = team.TeamCode,
                    Team_Name = team.TeamName,
                    Created_At = team.CreatedAt,
                    Added_By = addedby.Fullname,          
                };

                return Result.Success(results);

            }
        }
    }
}
