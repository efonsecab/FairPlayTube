using System;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.VisitorTracking
{
    /// <summary>
    /// Holds the information required to track visitors information/behavior
    /// </summary>
    public class VisitorTrackingModel
    {
        /// <summary>
        /// Id for the VisitorTracking record
        /// </summary>
        public long VisitorTrackingId { get; set; }
        /// <summary>
        /// Visisted Url
        /// </summary>
        [Url]
        public string VisitedUrl { get; set; }
        /// <summary>
        /// Logged In user Azure Ad B2C Object Id
        /// </summary>
        public string UserAzureAdB2cObjectId { get; set; }
        /// <summary>
        /// Id for the User's Session
        /// </summary>
        public Guid SessionId { get; set; }
        /// <summary>
        /// Time in page
        /// </summary>
        public TimeSpan TimeOnPage { get; set; }
    }
}
