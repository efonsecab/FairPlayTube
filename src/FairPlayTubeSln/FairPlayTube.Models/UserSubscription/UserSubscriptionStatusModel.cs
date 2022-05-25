using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserSubscription
{
    /// <summary>
    /// Holds the information for an user subscription status
    /// </summary>
    public class UserSubscriptionStatusModel
    {
        /// <summary>
        /// Application User Id
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// Subscription Plan Id
        /// </summary>
        public short SubscriptionPlanId { get; set; }
        /// <summary>
        /// Max Allowed Weekly Videos
        /// </summary>
        public short? MaxAllowedWeeklyVideos { get; set; }
        /// <summary>
        /// Videos Uploaded in the last 7 days
        /// </summary>
        public int UploadedVideosLast7Days { get; set; }
    }
}
