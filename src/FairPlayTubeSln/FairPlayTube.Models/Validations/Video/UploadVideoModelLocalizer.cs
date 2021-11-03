using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Validations.Video
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadVideoModelLocalizer
    {
        /// <summary>
        /// Types localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<UploadVideoModelLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the description required localized message
        /// </summary>
        public static string VideoDescriptionRequired => Localizer[VideoDescriptionRequiredTextKey];
        /// <summary>
        /// Retrieves the description too long localized message
        /// </summary>
        public static string DescriptionTooLong => Localizer[DescriptionTooLongTextKey];
        /// <summary>
        /// Retrieves the video name required localized message
        /// </summary>
        public static string VideoNameRequired => Localizer[VideoNameRequiredTextKey];
        /// <summary>
        /// Retrieves the name too long localized message
        /// </summary>
        public static string NameTooLong => Localizer[NameTooLongTextKey];
        /// <summary>
        /// Retrieves the invalida name format localized message
        /// </summary>
        public static string InvalidNameFormat => Localizer[InvalidNameFormatTextKey];
        /// <summary>
        /// Retrieves the invalid url format localized message
        /// </summary>
        public static string InvalidUrlFormat => Localizer[InvalidUrlFormatTextKey];
        /// <summary>
        /// Retrieves the url too long localized message
        /// </summary>
        public static string UrlTooLong => Localizer[UrlTooLongTextKey];
        /// <summary>
        /// Retrieves the price required localized message
        /// </summary>
        public static string PriceRequired => Localizer[PriceRequiredTextKey];
        /// <summary>
        /// Retrieves the price range localized message
        /// </summary>
        public static string PriceRange => Localizer[PriceRangeTextKey];
        /// <summary>
        /// Retrieves the language required localized message
        /// </summary>
        public static string LanguageRequired => Localizer[LanguageRequiredTextKey];
        /// <summary>
        /// Retrieves the video visibility required localized message
        /// </summary>
        public static string VideoVisibilityRequired => Localizer[VideoVisibilityRequiredTextKey];
        /// <summary>
        /// Resource key for video name required
        /// </summary>
        [ResourceKey(defaultValue:"Video name is required")]
        public const string VideoNameRequiredTextKey = "VideoNameRequiredText";
        /// <summary>
        /// Resource key for name too long
        /// </summary>
        [ResourceKey(defaultValue:"Name must be shorter than {1} characters")]
        public const string NameTooLongTextKey = "NameTooLongText";
        /// <summary>
        /// Resource key for invalid name format text
        /// </summary>
        [ResourceKey(defaultValue: "Name can only contain letters, numbers and spaces")]
        public const string InvalidNameFormatTextKey = "InvalidNameFormatText";
        /// <summary>
        /// Resource key for video description required
        /// </summary>
        [ResourceKey(defaultValue: "Video description is required")]
        public const string VideoDescriptionRequiredTextKey = "VideoDescriptionRequiredText";
        /// <summary>
        /// Resource key for description too long
        /// </summary>
        [ResourceKey(defaultValue: "Description must be shorter than {1} characters")]
        public const string DescriptionTooLongTextKey = "DescriptionTooLongText";
        /// <summary>
        /// Resource key for invalid url format
        /// </summary>
        [ResourceKey(defaultValue: "The url must have a valid format")]
        public const string InvalidUrlFormatTextKey = "InvalidUrlFormatText";
        /// <summary>
        /// Resource key for url too long
        /// </summary>
        [ResourceKey(defaultValue:"The Url must be shorter than {1} characters")]
        public const string UrlTooLongTextKey = "UrlTooLongText";
        /// <summary>
        /// Resource key for price required
        /// </summary>
        [ResourceKey(defaultValue:"Video's price is required")]
        public const string PriceRequiredTextKey = "PriceRequiredText";
        /// <summary>
        /// Resource key for price range
        /// </summary>
        [ResourceKey(defaultValue: "Price must be between {1} and {2}")]
        public const string PriceRangeTextKey = "PriceRangeText";
        /// <summary>
        /// Resource key for language required
        /// </summary>
        [ResourceKey(defaultValue: "Language is required")]
        public const string LanguageRequiredTextKey = "LanguageRequiredText";
        /// <summary>
        /// Resource key for video visibility required
        /// </summary>
        [ResourceKey(defaultValue:"Video visibility is required")]
        public const string VideoVisibilityRequiredTextKey = "VideoVisibilityRequiredText";
    }
}
