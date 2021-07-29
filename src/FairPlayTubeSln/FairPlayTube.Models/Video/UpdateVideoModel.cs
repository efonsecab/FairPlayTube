using FairPlayTube.Common.Global;
using System;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the information required to update a video
    /// </summary>
    public class UpdateVideoModel
    {
        /// <summary>
        /// Video's price
        /// </summary>
        [Required]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice)]
        public int Price { get; set; }
    }
}
