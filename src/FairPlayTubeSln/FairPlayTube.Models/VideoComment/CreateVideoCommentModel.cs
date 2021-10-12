using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VideoComment
{
    /// <summary>
    /// Holds the information required to create a comment for a specified video
    /// </summary>
    public class CreateVideoCommentModel
    {
        /// <summary>
        /// Id the comment is for
        /// </summary>
        [Required]
        public string VideoId { get; set; }
        /// <summary>
        /// Comment to be created
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
    }
}
