using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class UpdateTeamStatus
    {
        public class UpdateTeamStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateTeamStatusCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateTeamStatusCommand command, CancellationToken cancellationToken)
            {
                var teamExist = await _context.Teams.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (teamExist == null)
                {
                    return Result.Failure(TeamError.TeamNotExist());
                }

                teamExist.IsActive = !teamExist.IsActive;   

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
