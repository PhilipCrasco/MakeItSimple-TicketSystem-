using Microsoft.AspNetCore.SignalR;

namespace MakeItSimple.WebApi
{
    public interface IHubCaller
    {
        Task SendNotificationAsync(Guid userId, object message);
        Task AddUserToGroupAsync(string connectionId, Guid userId);
        Task RemoveUserFromGroupAsync(string connectionId, Guid userId);
    }


    public class HubCaller : IHubCaller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public HubCaller(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Guid userId, object message)
        {
            await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task AddUserToGroupAsync(string connectionId, Guid userId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, userId.ToString());
        }

        public async Task RemoveUserFromGroupAsync(string connectionId, Guid userId)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, userId.ToString());
        }

    }
}
