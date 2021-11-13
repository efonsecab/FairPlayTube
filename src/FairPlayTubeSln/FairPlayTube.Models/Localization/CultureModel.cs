using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Localization
{
    /// <summary>
    /// Holds the data for a supported culture
    /// </summary>
    public class CultureModel
    {
        /// <summary>
        /// Short name for the culture
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Display name for the culture
        /// </summary>
        public string DisplayName { get; set; }
    }
}
