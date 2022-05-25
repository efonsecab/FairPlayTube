using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.SubscriptionPlan
{
    /// <summary>
    /// Holds the information for a Subscription Plan
    /// </summary>
    public class SubscriptionPlanModel
    {
        /// <summary>
        /// Subscription Plan Id
        /// </summary>
        public short SubscriptionPlanId { get; set; }
        /// <summary>
        /// Subscription Plan Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Subscription Plan Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Max allowed weekly videos
        /// </summary>
        public short? MaxAllowedWeeklyVideos { get; set; }
    }
}
