using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Validations.VideoJobApplications
{
    /// <summary>
    /// Holds the logic required to retrieve the localized values for <see cref="CreateVideoJobApplicationLocalizer"/>
    /// </summary>
    public class CreateVideoJobApplicationLocalizer
    {
        /// <summary>
        /// Typed localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<CreateVideoJobApplicationLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the applicant cover letter required localized message
        /// </summary>
        public static string ApplicantCoverLetterRequired => Localizer[ApplicantCoverLetterRequiredTextKey];
        /// <summary>
        /// Retrieves video job id required localized message
        /// </summary>
        public static string VideoJobIdRequired => Localizer[VideoJobIdRequiredTextKey];
        /// <summary>
        /// Retrieves applicant cover letter localized display name
        /// </summary>
        public static string ApplicantCoverLetterDisplayName => Localizer[ApplicantCoverLetterDisplayNameTextKey];
        #region Resource Keys
        /// <summary>
        /// Resource key for application cover letter required
        /// </summary>
        [ResourceKey(defaultValue:"Applicant Cover Letter is required")]
        public const string ApplicantCoverLetterRequiredTextKey = "ApplicantCoverLetterRequiredText";
        /// <summary>
        /// Resource key for video job is required
        /// </summary>
        [ResourceKey(defaultValue: "Video Job Id is required")]
        public const string VideoJobIdRequiredTextKey = "VideoJobIdRequiredText";
        /// <summary>
        /// Resource key for applicant cover letter display name
        /// </summary>
        [ResourceKey(defaultValue: "Applicant Cover Letter")]
        public const string ApplicantCoverLetterDisplayNameTextKey = "ApplicantCoverLetterDisplayNameText";
        #endregion Resource Keys
    }
}
