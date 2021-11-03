using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomLocalization
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadVideoModelLocalizer
    {
        /// <summary>
        /// 
        /// </summary>
        public static IStringLocalizer<UploadVideoModelLocalizer> Localizer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static string VideoDescriptionRequired => Localizer[VideoDescriptionRequiredTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string DescriptionTooLong => Localizer[DescriptionTooLongTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string VideoNameRequired => Localizer[VideoNameRequiredTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string NameTooLong => Localizer[NameTooLongTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string InvalidNameFormat => Localizer[InvalidNameFormatTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string InvalidUrlFormat => Localizer[InvalidUrlFormatTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string UrlTooLong => Localizer[UrlTooLongTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string PriceRquired => Localizer[PriceRquiredTextKey];
        /// <summary>
        /// 
        /// </summary>
        public static string PriceRange => Localizer[PriceRangeTextKey];
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue:"Video name is required")]
        public const string VideoNameRequiredTextKey = "VideoNameRequiredText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue:"Name must be shorter than {1} characters")]
        public const string NameTooLongTextKey = "NameTooLongText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue: "Name can only contain letters, numbers and spaces")]
        public const string InvalidNameFormatTextKey = "InvalidNameFormatText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue: "Video description is required")]
        public const string VideoDescriptionRequiredTextKey = "VideoDescriptionRequiredText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue: "Description must be shorter than {1} characters")]
        public const string DescriptionTooLongTextKey = "DescriptionTooLongText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue: "The url must have a valid format")]
        public const string InvalidUrlFormatTextKey = "InvalidUrlFormatText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue:"The Url must be shorter than {1} characters")]
        public const string UrlTooLongTextKey = "UrlTooLongText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue:"Video's price is required")]
        public const string PriceRquiredTextKey = "PriceRquiredText";
        /// <summary>
        /// 
        /// </summary>
        [ResourceKey(defaultValue: "Price must be between {1} and {2}")]
        public const string PriceRangeTextKey = "PriceRangeText";
    }
}
