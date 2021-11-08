using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserYouTubeChannel
{
    /// <summary>
    /// Represents a YouTube video. From the YouTube Data API
    /// </summary>
    public class YouTubeVideoModel
    {
        /// <summary>
        /// Kind
        /// </summary>
        public string kind { get; set; }
        /// <summary>
        /// Etag
        /// </summary>
        public string etag { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public Id id { get; set; }
        /// <summary>
        /// Snippet
        /// </summary>
        public Snippet snippet { get; set; }
    }

    /// <summary>
    /// Represents an Id
    /// </summary>
    public class Id
    {
        /// <summary>
        /// Kind
        /// </summary>
        public string kind { get; set; }
        /// <summary>
        /// Video Id
        /// </summary>
        public string videoId { get; set; }
    }

    /// <summary>
    /// Represetns an Snippet
    /// </summary>
    public class Snippet
    {
        /// <summary>
        /// DateTime published
        /// </summary>
        public DateTime publishedAt { get; set; }
        /// <summary>
        /// Channel Id
        /// </summary>
        public string channelId { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Thumbnail
        /// </summary>
        public Thumbnails thumbnails { get; set; }
        /// <summary>
        /// Channel Title
        /// </summary>
        public string channelTitle { get; set; }
        /// <summary>
        /// Live Broadcst Content
        /// </summary>
        public string liveBroadcastContent { get; set; }
        /// <summary>
        /// DateTime published
        /// </summary>
        public DateTime publishTime { get; set; }
    }

    /// <summary>
    /// Represents the Thumbnails
    /// </summary>
    public class Thumbnails
    {
        /// <summary>
        /// Default
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        public object _default { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        /// <summary>
        /// Medium
        /// </summary>
        public Medium medium { get; set; }
        /// <summary>
        /// High
        /// </summary>
        public High high { get; set; }
    }

    /// <summary>
    /// Represents a Medium Thumbnail
    /// </summary>
    public class Medium
    {
        /// <summary>
        /// Url
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public int height { get; set; }
    }

    /// <summary>
    /// Represents a high Thumbnail
    /// </summary>
    public class High
    {
        /// <summary>
        /// Url
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public int height { get; set; }
    }
}
