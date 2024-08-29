using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MakeItSimple.WebApi
{
    public class NotificationHub : Hub
    {
        private readonly IHubCaller _hubCaller;

        public NotificationHub(IHubCaller hubCaller) 
        {
            _hubCaller = hubCaller;
        }

        public override async Task OnConnectedAsync()
        {

            if (Context.User.Identity is ClaimsIdentity identity &&
                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
            {
                await _hubCaller.AddUserToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity is ClaimsIdentity identity &&
                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
            {
                await _hubCaller.RemoveUserFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await _hubCaller.SendNotificationAsync(Guid.Parse(userId), message);
            }
        }


    }
}
