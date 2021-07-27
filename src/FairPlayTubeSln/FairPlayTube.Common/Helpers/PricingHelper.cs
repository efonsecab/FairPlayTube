using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Helpers
{
    public static class PricingHelper
    {
        public static decimal CalculateVideoAccessTotalPrice(decimal videoPrice)
        {
            var defaultCommision = Global.Constants.Commissions.VideoAccess;
            var totalPrice = videoPrice + (videoPrice * defaultCommision);
            return totalPrice;
        }
    }
}
