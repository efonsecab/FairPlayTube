using FairPlayTube.Models.Validations.VideoJobApplications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VideoJobApplications
{
    /// <summary>
    /// Holds the data for a video job application
    /// </summary>
    public class CreateVideoJobApplicationModel
    {
        /// <summary>
        /// Video job id
        /// </summary>
        public long VideoJobId { get; set; }
        /// <summary>
        /// Applicant's cover letter
        /// </summary>
        [Required(ErrorMessageResourceName =nameof(CreateVideoJobApplicationLocalizer.ApplicantCoverLetterRequired),
            ErrorMessageResourceType =typeof(CreateVideoJobApplicationLocalizer))]
        public string ApplicantCoverLetter { get; set; }
    }
}
