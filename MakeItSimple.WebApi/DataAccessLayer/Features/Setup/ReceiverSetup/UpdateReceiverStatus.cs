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
            public Guid ? UserId { get; set; }
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
                var receiver = await _context.Receivers.FirstOrDefaultAsync(x => x.UserId == command.UserId);

                if (receiver == null)
                {
                    return Result.Failure(ReceiverError.ReceiverNotExist());
                }

                var receiverList = await _context.Receivers.Where(x => x.UserId == receiver.UserId).ToListAsync();

                foreach(var receiverById in receiverList)
                {
                    receiverById.IsActive = !receiverById.IsActive;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
