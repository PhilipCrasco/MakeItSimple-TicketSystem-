using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.TeamSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class UpsertTeam
    {
        public class UpsertTeamCommand : IRequest<Result>
        {
            public int? SubUnitId { get; set; }
            public int? TeamId { get; set; }

            public string Team_Name { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

        }

        public class Handler : IRequestHandler<UpsertTeamCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertTeamCommand command, CancellationToken cancellationToken)
            {
                var subUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);
                if (subUnitNotExist == null)
                {
                    return Result.Failure(TeamError.SubUnitNotExist());
                }

                var teamExist = await _context.Teams.FirstOrDefaultAsync(x => x.Id == command.TeamId, cancellationToken);
                if (teamExist != null)
                {
                    var teamNameAlreadyExist = await _context.Teams.FirstOrDefaultAsync(x => x.TeamName == command.Team_Name 
                    && x.SubUnitId == command.SubUnitId, cancellationToken);

                    if (teamNameAlreadyExist != null && teamExist.TeamName != command.Team_Name)
                    {
                        return Result.Failure(TeamError.TeamNameAlreadyExist());
                    }


                    bool hasChange = false;

                    if(teamExist.SubUnitId != command.SubUnitId)
                    {
                        teamExist.SubUnitId = command.SubUnitId;    
                        hasChange = true;
                    }

                    if(teamExist.TeamName != command.Team_Name)
                    {
                        teamExist.TeamName = command.Team_Name;
                        hasChange = true;
                    }

                    if(hasChange)
                    {
                        teamExist.ModifiedBy = command.Modified_By;
                        teamExist.UpdatedAt = DateTime.Now;
                    }
      

                }
                else
                {
                    var teamNameAlreadyExist = await _context.Teams.FirstOrDefaultAsync(x => x.TeamName == command.Team_Name
                    && x.SubUnitId == command.SubUnitId, cancellationToken);

                    if (teamNameAlreadyExist != null)
                    {
                        return Result.Failure(TeamError.TeamNameAlreadyExist());
                    }

                    var addTeam = new Team
                    {
                        SubUnitId = command.SubUnitId,
                        TeamName = command.Team_Name,
                        AddedBy = command.Added_By,

                    };

                    await _context.Teams.AddAsync(addTeam);


                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();

            }
        }

    }
}
