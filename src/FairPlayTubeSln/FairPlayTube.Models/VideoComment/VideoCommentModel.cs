using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VideoComment
{
    /// <summary>
    /// Holds the information related to a video's comment
    /// </summary>
    public class VideoCommentModel
    {
        /// <summary>
        /// Id for a video's comment
        /// </summary>
        public long VideoCommentId { get; set; }
        /// <summary>
        /// VideoInfoId for the commented video
        /// </summary>
        public long VideoInfoId { get; set; }
        /// <summary>
        /// Id of the user who created the comment
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// Comment's text
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Date and Time of comment's creation
        /// </summary>
        public DateTimeOffset RowCreationDateTime { get; set; }
        /// <summary>
        /// User who created the comment
        /// </summary>
        public string RowCreationUser { get; set; }
        /// <summary>
        /// Number of Followers for the user who created the comment
        /// </summary>
        public long ApplicationUserFollowersCount { get; set; }
    }
}
