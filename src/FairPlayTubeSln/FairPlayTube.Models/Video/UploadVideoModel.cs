using FairPlayTube.Common.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        [Required]
        public string SourceUrl { get; set; }
        /// <summary>
        /// Video's Price
        /// </summary>
        [Required]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice)]
        public int Price { get; set; }
    }
}
