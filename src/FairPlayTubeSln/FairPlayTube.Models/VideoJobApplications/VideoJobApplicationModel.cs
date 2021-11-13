using FairPlayTube.Models.VideoJobApplications.Localizers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VideoJobApplications
{
    /// <summary>
    /// Holds the data related to a video job application
    /// </summary>
    public class VideoJobApplicationModel
    {
        /// <summary>
        /// Video Job Application Id
        /// </summary>
        public long VideoJobApplicationId { get; set; }
        /// <summary>
        /// Video Job Id
        /// </summary>
        public long VideoJobId { get; set; }
        /// <summary>
        /// Applicant Application User Id
        /// </summary>
        public long ApplicantApplicationUserId { get; set; }
        /// <summary>
        /// Applicant Cover Letter
        /// </summary>
        [Display(Name = nameof(VideoJobApplicationLocalizer.ApplicantCoverLetterDisplayName),
            ResourceType = typeof(VideoJobApplicationLocalizer))]
        public string ApplicantCoverLetter { get; set; }
        /// <summary>
        /// Video Job Application Status Id
        /// </summary>
        public short VideoJobApplicationStatusId { get; set; }
        /// <summary>
        /// UTC DateTime when the application was received
        /// </summary>
        public DateTimeOffset RowCreationDateTime { get; set; }
        /// <summary>
        /// Full name of user who created the application
        /// </summary>
        [Display(Name = nameof(VideoJobApplicationLocalizer.ApplicantNameDisplayName),
            ResourceType = typeof(VideoJobApplicationLocalizer))]
        public string RowCreationUser { get; set; }
        /// <summary>
        /// Title of the Video Job
        /// </summary>
        [Display(Name = nameof(VideoJobApplicationLocalizer.VideoJobTitleDisplayName),
            ResourceType = typeof(VideoJobApplicationLocalizer))]
        public string VideoJobTitle { get; set; }
        /// <summary>
        /// Description of the Video Job
        /// </summary>
        [Display(Name = nameof(VideoJobApplicationLocalizer.VideoJobDescriptionDisplayName),
            ResourceType = typeof(VideoJobApplicationLocalizer))]
        public string VideoJobDescription { get; set; }
    }
}
