namespace FairPlayTube.Models.Notifications
{
    /// <summary>
    /// Rpresents a Notification used in the SignalR communication
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Message of the SignalR notification
        /// </summary>
        public string Message { get; set; }
    }
}
