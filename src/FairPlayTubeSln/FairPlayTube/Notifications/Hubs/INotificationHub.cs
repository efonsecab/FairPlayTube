using FairPlayTube.Models.Notifications;
using System.Threading.Tasks;

namespace FairPlayTube.Notifications.Hubs
{
    public interface INotificationHub
    {
        Task ReceiveMessage(NotificationModel model);
    }
}