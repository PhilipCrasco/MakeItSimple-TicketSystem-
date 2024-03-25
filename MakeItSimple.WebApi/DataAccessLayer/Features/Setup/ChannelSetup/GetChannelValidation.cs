using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup
{
    public class GetChannelValidation
    {

        public class GetChannelValidationCommand : IRequest<Result>
        {
            public string Channel_Name { get; set; }
        }


        public class Handler : IRequestHandler<GetChannelValidationCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetChannelValidationCommand command, CancellationToken cancellationToken)
            {
                var channels = await _context.Channels.FirstOrDefaultAsync(x => x.ChannelName == command.Channel_Name);
                if(channels != null)
                {
                    return Result.Failure(ChannelError.ChannelNameAlreadyExist(command.Channel_Name));
                }

                return Result.Success("Tama kana bai!");

            }
        }
    }
}
