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
