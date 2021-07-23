using FairPlayTube.Common.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    public class UpdateVideoModel
    {
        [Required]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice)]
        public int Price { get; set; }
    }
}
