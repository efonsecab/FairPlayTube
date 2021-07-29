using FairPlayTube.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FairPlayTube.Notifications.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        public async Task SendMessage(NotificationModel model)
        {
            await this.Clients.All.ReceiveMessage(model);
        }

        public Task SendMessageToCaller(NotificationModel model)
        {
            return Clients.Caller.ReceiveMessage(model);
        }
    }
}
