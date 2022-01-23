﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(VideoInfoId), nameof(BuyerApplicationUserId), Name = "UI_VideoAccessTransaction_BuyerApplicationUserIdVideoInfoId", IsUnique = true)]
    public partial class VideoAccessTransaction
    {
        [Key]
        public long VideoAccessTransactionId { get; set; }
        public long VideoInfoId { get; set; }
        public long BuyerApplicationUserId { get; set; }
        [Column(TypeName = "money")]
        public decimal AppliedPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal AppliedCommission { get; set; }
        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }

        [ForeignKey(nameof(BuyerApplicationUserId))]
        [InverseProperty(nameof(ApplicationUser.VideoAccessTransaction))]
        public virtual ApplicationUser BuyerApplicationUser { get; set; }
        [ForeignKey(nameof(VideoInfoId))]
        [InverseProperty("VideoAccessTransaction")]
        public virtual VideoInfo VideoInfo { get; set; }
    }
}