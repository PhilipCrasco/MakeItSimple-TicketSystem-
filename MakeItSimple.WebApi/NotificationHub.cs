using Microsoft.AspNetCore.SignalR;

namespace MakeItSimple.WebApi
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }

}
