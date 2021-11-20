using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UsersRequests.Localizers
{
    /// <summary>
    /// Holds the logic required to retrieve the localized values for <see cref="CreateUserRequestModel"/>
    /// </summary>
    public class CreateUserRequestModelLocalizer
    {
        /// <summary>
        /// Typed localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<CreateUserRequestModelLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the description required localized value
        /// </summary>
        public static string DescriptionRequired => Localizer[DescriptionRequiredTextKey];
        /// <summary>
        /// Retrieves the description too long localized value
        /// </summary>
        public static string DescriptionTooLong => Localizer[DescriptionTooLongTextKey];
        /// <summary>
        /// Retrieves the email address required localized value
        /// </summary>
        public static string EmailAddressRequired => Localizer[EmailAddressRequiredTextKey];
        /// <summary>
        /// Retrieves the email address too long localized value
        /// </summary>
        public static string EmailAddressTooLong => Localizer[EmailAddressTooLongTextKey];
        /// <summary>
        /// Retrieves the email address format localized value
        /// </summary>
        public static string EmailAddressFormat => Localizer[EmailAddressFormatTextKey];
        #region Resource Keys
        /// <summary>
        /// Resource keys for description required
        /// </summary>
        [ResourceKey(defaultValue:"{0} is required")]
        public const string DescriptionRequiredTextKey = "DescriptionRequiredText";
        /// <summary>
        /// Resource key for description too long
        /// </summary>
        [ResourceKey(defaultValue:"{0} must be shorted than {1} characters")]
        public const string DescriptionTooLongTextKey = "DescriptionTooLongText";
        /// <summary>
        /// Resource key for email address required
        /// </summary>
        [ResourceKey(defaultValue:"{0} is required")]
        public const string EmailAddressRequiredTextKey = "EmailAddressRequiredText";
        /// <summary>
        /// Resource key for email address too long
        /// </summary>
        [ResourceKey(defaultValue:"{0} must be shorter than {1} characters")]
        public const string EmailAddressTooLongTextKey = "EmailAddressTooLongText";
        /// <summary>
        /// Resource key for email address format
        /// </summary>
        [ResourceKey(defaultValue:"{0} must have a valid email format")]
        public const string EmailAddressFormatTextKey = "EmailAddressFormatText";
        #endregion Resource Keys
    }
}
