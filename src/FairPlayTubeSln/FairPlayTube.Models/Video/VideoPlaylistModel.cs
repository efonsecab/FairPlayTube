using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the information related to a video's playlist
    /// </summary>
    public class VideoPlaylistModel
    {
        /// <summary>
        /// Playlist's name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PlaylistName { get; set; }
        /// <summary>
        /// Playlist's description
        /// </summary>
        [Required]
        [StringLength(250)]
        public string PlaylistDescription { get; set; }
    }
}
