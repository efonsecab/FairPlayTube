using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Payouts
{
    /// <summary>
    /// Holds the data for a Paypal Payout Batch Item
    /// </summary>
    public class PaypalPayoutBatchItemModel
    {
        /// <summary>
        /// Paypal Payout Batch Id
        /// </summary>
        public long PaypalPayoutBatchId { get; set; }
        /// <summary>
        /// Rcipient Type
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RecipientType { get; set; }
        /// <summary>
        /// Amount value
        /// </summary>
        public decimal AmountValue { get; set; }
        /// <summary>
        /// Amount currency
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AmountCurrency { get; set; }
        /// <summary>
        /// Note
        /// </summary>
        [Required]
        public string Note { get; set; }
        /// <summary>
        /// Sender Item Id
        /// </summary>
        public Guid SenderItemId { get; set; }
        /// <summary>
        /// Receiver Email Address
        /// </summary>
        [Required]
        [StringLength(150)]
        public string ReceiverEmailAddress { get; set; }
        /// <summary>
        /// Notification Laguage
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NotificationLanguage { get; set; }
    }
}
