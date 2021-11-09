namespace FairPlayTube.Models.Paypal
{
#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    /// Represents the PayPal Checkout Approval Data
    /// </summary>
    public class PaypalCheckoutApprovedDataModel
    {
        /// <summary>
        /// Order Id
        /// </summary>
        public string orderID { get; set; }
        /// <summary>
        /// Payer Id
        /// </summary>
        public string payerID { get; set; }
        /// <summary>
        /// Payment Id
        /// </summary>
        public object paymentID { get; set; }
        /// <summary>
        /// Billing Token
        /// </summary>
        public object billingToken { get; set; }
        /// <summary>
        /// Facilitator Access Token
        /// </summary>
        public string facilitatorAccessToken { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
