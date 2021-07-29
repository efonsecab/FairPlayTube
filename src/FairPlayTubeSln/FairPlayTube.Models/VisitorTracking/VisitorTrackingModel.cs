using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VisitorTracking
{
    /// <summary>
    /// Holds the information required to track visitors information/behavior
    /// </summary>
    public class VisitorTrackingModel
    {
        /// <summary>
        /// Visisted Url
        /// </summary>
        [Url]
        public string VisitedUrl { get; set; }
        /// <summary>
        /// Logged In user Azure Ad B2C Object Id
        /// </summary>
        public string UserAzureAdB2cObjectId { get; set; }
    }
}
