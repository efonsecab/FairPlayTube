using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.Invites
{
    /// <summary>
    /// Represents an invitation to a user to use the system
    /// </summary>
    public class InviteUserModel
    {
        /// <summary>
        /// Email Address the invitation is sent to
        /// </summary>
        [Required]
        [EmailAddress]
        public string ToEmailAddress { get; set; }
        /// <summary>
        /// Custom message for the invitation
        /// </summary>
        [Required]
        public string CustomMessage { get; set; }
    }
}
