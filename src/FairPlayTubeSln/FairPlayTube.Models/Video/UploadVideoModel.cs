using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the information required to upload a new video
    /// </summary>
    public class UploadVideoModel
    {
        /// <summary>
        /// Name/Title for the video
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Video's Description
        /// </summary>
        [StringLength(500)]
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Public Url where the source video is located
        /// </summary>
        [Url]
        [StringLength(500)]
        public string SourceUrl { get; set; }
        /// <summary>
        /// Video's Price
        /// </summary>
        [Required]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice)]
        public int Price { get; set; }

        /// <summary>
        /// The video's language
        /// </summary>
        [Required]
        public string Language { get; set; }
        /// <summary>
        /// The video's visibility
        /// </summary>
        [Required]
        public VideoVisibility VideoVisibility { get; set; } = VideoVisibility.Public;
        /// <summary>
        /// Generated unique name
        /// </summary>
        public string StoredFileName { get; set; }
        /// <summary>
        /// Indicate the user will specify a Source Url instead of uploading a file
        /// </summary>
        public bool UseSourceUrl { get; set; }
    }
}
