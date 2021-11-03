using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Models.Validations.Video;
using System;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the information required to upload a new video
    /// </summary>
    public class UploadVideoModel
    {
        /// <summary>
        /// Name/Title for the video
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.VideoNameRequired),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        [StringLength(50, ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.NameTooLong),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        [RegularExpression(Constants.RegularExpressions.AllowedFileNameFormat,
            ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.InvalidNameFormat),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer)
            )]
        public string Name { get; set; }
        /// <summary>
        /// Video's Description
        /// </summary>
        [StringLength(500, ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.DescriptionTooLong),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        [Required(ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.VideoDescriptionRequired),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        public string Description { get; set; }
        /// <summary>
        /// Public Url where the source video is located
        /// </summary>
        [Url(ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.InvalidUrlFormat),
            ErrorMessageResourceType = typeof(UploadVideoModel))]
        [StringLength(500, ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.UrlTooLong),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        public string SourceUrl { get; set; }
        /// <summary>
        /// Video's Price
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.PriceRequired),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        [Range(Constants.PriceLimits.MinVideoPrice, Constants.PriceLimits.MaxVideoPrice,

            ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.PriceRange),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        public int Price { get; set; }

        /// <summary>
        /// The video's language
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(UploadVideoModelLocalizer.LanguageRequired),
            ErrorMessageResourceType = typeof(UploadVideoModelLocalizer))]
        public string Language { get; set; }
        /// <summary>
        /// The video's visibility
        /// </summary>
        [Required(ErrorMessageResourceName =nameof(UploadVideoModelLocalizer.VideoVisibilityRequired),
            ErrorMessageResourceType =typeof(UploadVideoModelLocalizer))]
        public VideoVisibility VideoVisibility { get; set; } = VideoVisibility.Public;
        /// <summary>
        /// Generated unique name
        /// </summary>
        public string StoredFileName { get; set; }
        /// <summary>
        /// Indicate the user will specify a Source Url instead of uploading a file
        /// </summary>
        public bool UseSourceUrl { get; set; }
    }
}
