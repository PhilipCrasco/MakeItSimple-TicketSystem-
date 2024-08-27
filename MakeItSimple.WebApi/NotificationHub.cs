using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MakeItSimple.WebApi
{
    public class NotificationHub : Hub
    {
        private readonly IConfiguration _configuration;

        public NotificationHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userIdClaim = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(Guid userId, string message)
        {
            await Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
