using FairPlayTube.Models.Validations.Video;
using System;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Represents a Job associated to a video
    /// </summary>
    public class VideoJobModel
    {
        /// <summary>
        /// Video Id
        /// </summary>
        [Required]
        public string VideoId { get; set; }
        /// <summary>
        /// Budget
        /// </summary>
        [Display(Name = nameof(VideoJobModelLocalizer.BudgetDisplayName),
            ResourceType = typeof(VideoJobModelLocalizer))]
        public decimal Budget { get; set; }
        /// <summary>
        /// Job Title
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = nameof(VideoJobModelLocalizer.TitleDisplayName), 
            ResourceType =typeof(VideoJobModelLocalizer))]
        public string Title { get; set; }
        /// <summary>
        /// Job Description
        /// </summary>
        [Required]
        [StringLength(250)]
        [Display(Name = nameof(VideoJobModelLocalizer.DescriptionDisplayName),
            ResourceType = typeof(VideoJobModelLocalizer))]
        public string Description { get; set; }
        /// <summary>
        /// DateTime the job was created
        /// </summary>
        public DateTimeOffset RowCreationDateTime { get; set; }
        /// <summary>
        /// Related video
        /// </summary>
        public VideoInfoModel VideoInfo { get; set; }
    }
}
