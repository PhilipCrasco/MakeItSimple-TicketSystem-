using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification
{
    public class GetTicketTransactionNotification
    {
        private class TicketTransactionResult
        {
            public int Id { get; set; }
            public string Message { get; set; } 
            public string Added_By { get; set; }  
            public DateTime Created_At { get; set; }

            public string Receive_By { get; set; }
            public bool Is_Checked { get; set; }
            public string Modules { get; set; }
        }




        public class GetTicketTransactionNotificationCommand : IRequest<Result>
        {
            public Guid UserId{ get; set; }
            public string Role { get; set; }
           

        }

        public class Handler : IRequestHandler<GetTicketTransactionNotificationCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {                                                                                     
                _context = context;
            }

            public async Task<Result> Handle(GetTicketTransactionNotificationCommand request, CancellationToken cancellationToken)
            {

                var result = await _context.TicketTransactionNotifications
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ReceiveByUser)
                    .Where(x => x.ReceiveBy == request.UserId && x.IsChecked == false)
                    .Select(x => new TicketTransactionResult
                    {
                        Id = x.Id,
                        Message = x.Message,
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.Created_At,
                        Receive_By = x.ReceiveByUser.Fullname,
                        Is_Checked = x.IsChecked,
                        Modules = x.Modules

                    }).OrderByDescending(x => x.Created_At)
                    .ToListAsync();

                return Result.Success(result);
            }


        }
    }
}
