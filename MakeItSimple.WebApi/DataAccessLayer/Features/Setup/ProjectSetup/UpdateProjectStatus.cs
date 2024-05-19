using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup
{
    public class UpdateProjectStatus
    {
        public class UpdateProjectStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateProjectStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateProjectStatusCommand command, CancellationToken cancellationToken)
            {
                var projectExist = await _context.Projects.FirstOrDefaultAsync(x => x.Id == command.Id ,cancellationToken);
                if (projectExist == null)
                {
                    return Result.Failure(ChannelError.ProjectNotExist());
                }

                projectExist.IsActive = !projectExist.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
