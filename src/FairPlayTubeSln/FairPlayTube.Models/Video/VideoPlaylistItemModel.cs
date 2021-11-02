using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Hold the data related to a video's playlist item
    /// </summary>
    public class VideoPlaylistItemModel
    {
        /// <summary>
        /// Playlist Item Id
        /// </summary>
        public long VideoPlaylistItemId { get; set; }
        /// <summary>
        /// Playlist Id
        /// </summary>
        public long VideoPlaylistId { get; set; }
        /// <summary>
        /// Video's Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// Order of item in playlist
        /// </summary>
        public int Order { get; set; }
    }
}
