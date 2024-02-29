using Microsoft.AspNetCore.SignalR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.SignalRNotification
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("RecieveMessage", $"{connectionId} has joined");
        }

        public async Task SendingMessage(string Message)
        {
            string connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("SendingMessage", $"{connectionId}: {Message}");
        }

    }
}
