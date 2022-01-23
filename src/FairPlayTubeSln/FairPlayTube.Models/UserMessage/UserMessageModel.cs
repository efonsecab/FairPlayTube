using System;
using System.ComponentModel.DataAnnotations;

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
        /// Full Name of the To Application User
        /// </summary>
        public string ToApplicationUserFullName { get; set; }
        /// <summary>
        /// Full Name of the From Application User
        /// </summary>
        public string FromApplicationUserFullName { get; set; }
        /// <summary>
        /// Message to be sent
        /// </summary>
        [Required]
        public string Message { get; set; }
        /// <summary>
        /// UTC DateTime the message was created
        /// </summary>
        public DateTimeOffset? RowCreationDateTime { get; set; }
        /// <summary>
        /// Indicated if the message has been read by the destinatary
        /// </summary>
        public bool ReadByDestinatary { get; set; }
    }
}
