using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the bytes that composed the video with the given VideoId
    /// </summary>
    public class DownloadVideoModel
    {
        /// <summary>
        /// VideoId
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// File bytes
        /// </summary>
        public byte[] VideoBytes { get; set; }
    }
}
