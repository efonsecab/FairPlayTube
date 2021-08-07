using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserAudience
{
    /// <summary>
    /// Represents a follower for a given user
    /// </summary>
    public class UserFollowerModel
    {
        /// <summary>
        /// Id for the user following
        /// </summary>
        public long FollowedApplicationUserId { get; set; }
        /// <summary>
        /// Id for the user being followed
        /// </summary>
        public long FollowerApplicationUserId { get; set; }
    }
}
