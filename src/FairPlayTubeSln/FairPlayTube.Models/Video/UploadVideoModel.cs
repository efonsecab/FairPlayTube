using FairPlayTube.Common.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    public class UploadVideoModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(500)]
        [Required]
        public string Description { get; set; }
        [Url]
        [StringLength(500)]
        [Required]
        public string SourceUrl { get; set; }
        [Required]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice)]
        public int Price { get; set; }
    }
}
