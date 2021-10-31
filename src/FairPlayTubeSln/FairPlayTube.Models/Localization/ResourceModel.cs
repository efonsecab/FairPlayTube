using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Localization
{
    /// <summary>
    /// Represents the culture
    /// </summary>
    public class ResourceModel
    {
        /// <summary>
        /// The resource type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        [Required]
        public string Value { get; set; }
        /// <summary>
        /// Culture Name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CultureName { get; set; }
    }
}
