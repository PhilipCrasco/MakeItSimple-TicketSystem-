using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ReceiverSetup
{
    public class UpdateReceiverStatus
    {
        public class UpdateReceiverStatusCommand : IRequest<Result>
        {
            public int? Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateReceiverStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateReceiverStatusCommand command, CancellationToken cancellationToken)
            {
                var receiver = await _context.Receivers.FirstOrDefaultAsync(x => x.Id == command.Id);

                if (receiver == null)
                {
                    return Result.Failure(ReceiverError.ReceiverNotExist());
                }

                receiver.IsActive  = !receiver.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
