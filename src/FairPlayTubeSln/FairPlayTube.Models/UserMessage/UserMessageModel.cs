namespace FairPlayTube.Models.UserMessage
{
    /// <summary>
    /// Represents the User Message entry
    /// </summary>
    public class UserMessageModel
    {
        /// <summary>
        /// ApplicationUserId of the user to whom the message is sent
        /// </summary>
        public long ToApplicationUserId { get; set; }
        /// <summary>
        /// Message to be sent
        /// </summary>
        public string Message { get; set; }
    }
}
