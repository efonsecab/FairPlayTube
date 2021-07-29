using System.ComponentModel.DataAnnotations;

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
