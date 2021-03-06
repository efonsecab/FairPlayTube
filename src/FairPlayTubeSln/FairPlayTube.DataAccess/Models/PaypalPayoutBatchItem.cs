// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    public partial class PaypalPayoutBatchItem
    {
        public PaypalPayoutBatchItem()
        {
            VideoJobEscrow = new HashSet<VideoJobEscrow>();
        }

        [Key]
        public long PaypalPayoutBatchItemId { get; set; }
        public long PaypalPayoutBatchId { get; set; }
        [Required]
        [StringLength(50)]
        public string RecipientType { get; set; }
        [Column(TypeName = "money")]
        public decimal AmountValue { get; set; }
        [Required]
        [StringLength(50)]
        public string AmountCurrency { get; set; }
        [Required]
        [Column(TypeName = "text")]
        public string Note { get; set; }
        [Required]
        [StringLength(50)]
        public string SenderItemId { get; set; }
        [Required]
        [StringLength(150)]
        public string ReceiverEmailAddress { get; set; }
        [Required]
        [StringLength(50)]
        public string NotificationLanguage { get; set; }
        public DateTimeOffset RowCreationDateTime { get; set; }
        [Required]
        [StringLength(256)]
        public string RowCreationUser { get; set; }
        [Required]
        [StringLength(250)]
        public string SourceApplication { get; set; }
        [Required]
        [Column("OriginatorIPAddress")]
        [StringLength(100)]
        public string OriginatorIpaddress { get; set; }

        [ForeignKey(nameof(PaypalPayoutBatchId))]
        [InverseProperty("PaypalPayoutBatchItem")]
        public virtual PaypalPayoutBatch PaypalPayoutBatch { get; set; }
        [InverseProperty("PaypalPayoutBatchItem")]
        public virtual ICollection<VideoJobEscrow> VideoJobEscrow { get; set; }
    }
}