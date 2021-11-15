using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Features
{
    /// <summary>
    /// Holds the data related to a feature
    /// </summary>
    public class FeatureModel
    {
        /// <summary>
        /// Feature's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Enabled/Disabled status for the feature
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
