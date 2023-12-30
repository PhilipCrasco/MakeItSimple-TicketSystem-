using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class UpdateChannelStatus
    {

        public class UpdateChannelStatusResult
        {
            public int Id { get; set; }

            public bool Status { get; set; }

        }


        public class UpdateChannelStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }


        }

        public class Handler : IRequestHandler<UpdateChannelStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateChannelStatusCommand command, CancellationToken cancellationToken)
            {
                var channels = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (channels == null)
                {
                    return Result.Failure(ChannelError.ChannelNotExist());
                }

                channels.IsActive = !channels.IsActive;

                var results = new UpdateChannelStatusResult
                {
                   Id = channels.Id,
                   Status = channels.IsActive
                };

                return Result.Success(channels);

            }
        }
    }
}
