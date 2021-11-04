using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Payouts
{
    /// <summary>
    /// Holds the data for a paypal payout batch
    /// </summary>
    public class PaypalPayoutBatchModel
    {
        /// <summary>
        /// Sender Batch Id
        /// </summary>
        public Guid SenderBatchId { get; set; }
        /// <summary>
        /// Email Subject
        /// </summary>
        [Required]
        [StringLength(250)]
        public string EmailSubject { get; set; }
        /// <summary>
        /// Email Message
        /// </summary>
        [Required]
        public string EmailMessage { get; set; }
        /// <summary>
        /// Payout Batch Items
        /// </summary>
        public PaypalPayoutBatchItemModel[] PaypalPayoutBatchItem { get; set; }
    }
}
