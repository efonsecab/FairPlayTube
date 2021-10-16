using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserYouTubeChannel
{
    /// <summary>
    /// Holds the data related to a users YouTube channels
    /// </summary>
    public class UserYouTubeChannelModel
    {
        /// <summary>
        /// User's id
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// Youtube Channel Id
        /// </summary>
        [Required]
        [StringLength(50)]
        public string YouTubeChannelId { get; set; }
        /// <summary>
        /// DateTime the entry was created
        /// </summary>
        public DateTimeOffset RowCreationDateTime { get; set; }
    }
}
