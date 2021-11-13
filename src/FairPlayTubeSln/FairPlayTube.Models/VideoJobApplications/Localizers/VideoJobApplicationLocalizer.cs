using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VideoJobApplications.Localizers
{
    /// <summary>
    /// Holds the logic required to retrieve the localized values for <see cref="VideoJobApplicationModel"/>
    /// </summary>
    public class VideoJobApplicationLocalizer
    {
        /// <summary>
        /// Typed localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<VideoJobApplicationLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the Applicant Name locazlied message
        /// </summary>
        public static string ApplicantNameDisplayName => Localizer[ApplicantNameDisplayNameTextKey];
        /// <summary>
        /// Retrieves the applicant's cover letter localized Display Name
        /// </summary>
        public static string ApplicantCoverLetterDisplayName => Localizer[ApplicantCoverLetterDisplayNameTextKey];
        /// <summary>
        /// Retrieves the job title localized Display Name
        /// </summary>
        public static string VideoJobTitleDisplayName => Localizer[VideoJobTitleDisplayNameTextKey];
        /// <summary>
        /// Retrieves the job description localized Display Name
        /// </summary>
        public static string VideoJobDescriptionDisplayName => Localizer[VideoJobDescriptionDisplayNameTextKey];

        #region Resource Keys
        /// <summary>
        /// Resource key for the Applicant Name Display Name
        /// </summary>
        [ResourceKey(defaultValue: "Applicant Name")]
        public const string ApplicantNameDisplayNameTextKey = "ApplicantNameDisplayNameText";
        /// <summary>
        /// Resource key for the Applicant's Cover Letter Display Name
        /// </summary>
        [ResourceKey(defaultValue: "Cover Letter")]
        public const string ApplicantCoverLetterDisplayNameTextKey = "ApplicantCoverLetterDisplayNameText";
        /// <summary>
        /// Resource key for Video Job Title Display Name
        /// </summary>
        [ResourceKey(defaultValue:"Job Title")]
        public const string VideoJobTitleDisplayNameTextKey = "VideoJobTitleDisplayNameText";
        /// <summary>
        /// Resource key for Video Job Description Display Name
        /// </summary>
        [ResourceKey(defaultValue:"Job Description")]
        public const string VideoJobDescriptionDisplayNameTextKey = "VideoJobDescriptionDisplayNameText";
        #endregion Resource Keys
    }
}
