using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.Build.Execution;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Authentication
{
    public class LogoutAuthentication
    {
        public class LogOutAuthenticationResult
        {

            public Guid? Id { get; set; }

            public string Username { get; set; }

            public string Token { get; set; }

        }

        public class  LogoutAuthenticationCommand : IRequest<Result>
        {
            public Guid? Id { get; set; }
            public string Token { get; set; }
        }


        public class Handler : IRequestHandler<LogoutAuthenticationCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(LogoutAuthenticationCommand command, CancellationToken cancellationToken)
            {

                

                await _context.SaveChangesAsync(cancellationToken);

                var result = new LogOutAuthenticationResult();

                return Result.Success(result);

            }

        
        }


    }
}
