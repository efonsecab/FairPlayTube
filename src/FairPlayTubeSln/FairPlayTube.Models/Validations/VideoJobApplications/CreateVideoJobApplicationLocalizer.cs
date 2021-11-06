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
        #region Resource Keys
        /// <summary>
        /// Resource key for application cover letter required
        /// </summary>
        [ResourceKey(defaultValue:"Applicant Cover Letter is required")]
        public const string ApplicantCoverLetterRequiredTextKey = "ApplicantCoverLetterRequiredText";
        #endregion Resource Keys
    }
}
