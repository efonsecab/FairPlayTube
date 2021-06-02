using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserProfile
{
    public class GlobalMonetizationModel
    {
        [ValidateComplexType]
        public List<MonetizationItem> MonetizationItems { get; set; }
    }

    public class MonetizationItem
    {
        [Url]
        [Required]
        [StringLength(1000)]
        public string MonetizationUrl { get; set; }
    }
}
