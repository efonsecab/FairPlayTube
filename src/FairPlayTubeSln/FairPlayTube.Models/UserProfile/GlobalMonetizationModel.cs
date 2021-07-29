using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.UserProfile
{
    /// <summary>
    /// Represents the External Monetization Information
    /// </summary>
    public class GlobalMonetizationModel
    {
        /// <summary>
        /// List of Monetization Items
        /// </summary>
        [ValidateComplexType]
        public List<MonetizationItem> MonetizationItems { get; set; }
    }

    /// <summary>
    /// Represents a Monetization Item
    /// </summary>
    public class MonetizationItem
    {
        /// <summary>
        /// Url of the Monetization Item
        /// </summary>
        [Url]
        [Required]
        [StringLength(1000)]
        public string MonetizationUrl { get; set; }
    }
}
