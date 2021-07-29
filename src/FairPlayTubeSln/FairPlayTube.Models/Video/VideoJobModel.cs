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
        public decimal Budget { get; set; }
        /// <summary>
        /// Job Title
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        /// <summary>
        /// Job Description
        /// </summary>
        [Required]
        [StringLength(250)]
        public string Description { get; set; }
    }
}
