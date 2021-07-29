using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
