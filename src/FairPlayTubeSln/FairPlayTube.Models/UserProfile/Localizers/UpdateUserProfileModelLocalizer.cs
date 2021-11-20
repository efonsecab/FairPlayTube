using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserProfile.Localizers
{
    /// <summary>
    /// Holds the logic required to retrieve the localized values for <see cref="UpdateUserProfileModel"/>
    /// </summary>
    public class UpdateUserProfileModelLocalizer
    {
        /// <summary>
        /// Typed localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<UpdateUserProfileModelLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the localized display name for About field
        /// </summary>
        public static string AboutDisplayName => Localizer[AboutDisplayNameTextKey];
        /// <summary>
        /// Retrieves the localized display name for Alias field
        /// </summary>
        public static string AliasDisplayName => Localizer[AliasDisplayNameTextKey];
        /// <summary>
        /// Retrieves the Alias too long localized message
        /// </summary>
        public static string AliasTooLong => Localizer[AliasTooLongTextKey];
        /// <summary>
        /// Retrieves the localized display name for Paypal Email Address field
        /// </summary>
        public static string PaypalEmailAddressDisplayName => Localizer[PaypalEmailAddressDisplayNameTextKey];
        /// <summary>
        /// Retrieves the localized display name for National Id field
        /// </summary>
        public static string NationalIdNumberDisplayName => Localizer[NationalIdNumberDisplayNameTextKey];
        /// <summary>
        /// Retrieves the localized error message for invalid email address format in Paypal Email Address field
        /// </summary>
        public static string PaypalEmailAddressFormatError => Localizer[PaypalEmailAddressFormatErrorTextKey];
        /// <summary>
        /// Retrieves about required localized message
        /// </summary>
        public static string AboutRequired => Localizer[AboutRequiredTextKey];
        /// <summary>
        /// Retrieves about too long localized message
        /// </summary>
        public static string AboutTooLong => Localizer[AboutTooLongTextKey];
        /// <summary>
        /// Retrieves Topics required localized message
        /// </summary>
        public static string TopicsRequired => Localizer[TopicsRequiredTextKey];
        /// <summary>
        /// Retrieves the Topics too long localized message
        /// </summary>
        public static string TopicsTooLong => Localizer[TopicsTooLongTextKey];
        /// <summary>
        /// Retrieves the localized display name for Topics field
        /// </summary>
        public static string TopicsDisplayName => Localizer[TopicsDisplayNameTextKey];
        /// <summary>
        /// Retrieves the nationality required localized message
        /// </summary>
        public static string NationalityRequired => Localizer[NationalityRequiredTextKey];
        /// <summary>
        /// Retrieves the nationality too long localized message
        /// </summary>
        public static string NationalityTooLong => Localizer[NationalityTooLongTextKey];
        /// <summary>
        /// Retrieves the localized display name for Nationality field
        /// </summary>
        public static string NationalityDisplayName => Localizer[NationalityDisplayNameTextKey];
        /// <summary>
        /// Retrieives the National Id too long localized message
        /// </summary>
        public static string NationalIdTooLong => Localizer[NationalIdTooLongTextKey];

        #region Resource Keys
        /// <summary>
        /// Resource key for About Display Name
        /// </summary>
        [ResourceKey(defaultValue: "About")]
        public const string AboutDisplayNameTextKey = "AboutDisplayNameText";
        /// <summary>
        /// Resource key for Alias Display Name
        /// </summary>
        [ResourceKey(defaultValue:"Alias")]
        public const string AliasDisplayNameTextKey = "AliasDisplayNameText";
        /// <summary>
        /// Resource key for Alias too long
        /// </summary>
        [ResourceKey(defaultValue:"{0} must be shorter than {1} characters {1}")]
        public const string AliasTooLongTextKey = "AliasTooLongText";
        /// <summary>
        /// Resource key for Paypal Email Address Display Name
        /// </summary>
        [ResourceKey(defaultValue: "Paypal Email Address")]
        public const string PaypalEmailAddressDisplayNameTextKey = "PaypalEmailAddressDisplayNameText";
        /// <summary>
        /// Resource key for National Id NumberDisplay Name
        /// </summary>
        [ResourceKey(defaultValue: "National Id Number")]
        public const string NationalIdNumberDisplayNameTextKey = "NationalIdNumberDisplayNameText";
        /// <summary>
        /// Resource key for invalid format in Paypal Email Address
        /// </summary>
        [ResourceKey(defaultValue: "Paypal Email Address must have a valid Email Format")]
        public const string PaypalEmailAddressFormatErrorTextKey = "PaypalEmailAddressFormatErrorText";
        /// <summary>
        /// Resource key for About required
        /// </summary>
        [ResourceKey(defaultValue: "About is required")]
        public const string AboutRequiredTextKey = "AboutRequiredText";
        /// <summary>
        /// Resource key for About too long
        /// </summary>
        [ResourceKey(defaultValue: "About must be shorter than {1} characters")]
        public const string AboutTooLongTextKey = "AboutTooLongText";
        /// <summary>
        /// Resource key for Topics required
        /// </summary>
        [ResourceKey(defaultValue: "Topics is required")]
        public const string TopicsRequiredTextKey = "TopicsRequiredText";
        /// <summary>
        /// Resource keky for Topics too long
        /// </summary>
        [ResourceKey(defaultValue:"Topics must be shorter than {1} characters")]
        public const string TopicsTooLongTextKey = "TopicsTooLongText";
        /// <summary>
        /// Resource key for Topics Display Name
        /// </summary>
        [ResourceKey(defaultValue:"Topics")]
        public const string TopicsDisplayNameTextKey = "TopicsDisplayNameText";
        /// <summary>
        /// Resource key for Nationalitiy required
        /// </summary>
        [ResourceKey(defaultValue:"{0} is required")]
        public const string NationalityRequiredTextKey = "NationalityRequiredText";
        /// <summary>
        /// Resoure key for Nationality too long
        /// </summary>
        [ResourceKey(defaultValue:"{0} must be shorter than {1} characters")]
        public const string NationalityTooLongTextKey = "NationalityTooLongText";
        /// <summary>
        /// Resource key for Nationality dsplay name
        /// </summary>
        [ResourceKey(defaultValue: "Nationality")]
        public const string NationalityDisplayNameTextKey = "NationalityDisplayNameText";
        /// <summary>
        /// Resource keky for National Id too long
        /// </summary>
        [ResourceKey(defaultValue:"{0} must be shorter than {1} characters")]
        public const string NationalIdTooLongTextKey = "NationalIdTooLongText";
        #endregion Resource Keys
    }
}
