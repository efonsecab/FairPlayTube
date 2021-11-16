﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(PaypalEmailAddress), Name = "UI_UserProfile_PaypalEmailAddress", IsUnique = true)]
    public partial class UserProfile
    {
        [Key]
        public long UserProfileId { get; set; }
        public long ApplicationUserId { get; set; }
        public long UserVerificationStatusId { get; set; }
        [Required]
        [StringLength(1000)]
        public string About { get; set; }
        [Required]
        [StringLength(1000)]
        public string Topics { get; set; }
        [StringLength(20)]
        public string Nationality { get; set; }
        [StringLength(50)]
        public string NationalIdNumber { get; set; }
        [StringLength(200)]
        public string NationalIdPhotoUrl { get; set; }
        [StringLength(100)]
        public string DisplayAlias { get; set; }
        [StringLength(500)]
        public string PaypalEmailAddress { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [InverseProperty("UserProfile")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(UserVerificationStatusId))]
        [InverseProperty("UserProfile")]
        public virtual UserVerificationStatus UserVerificationStatus { get; set; }
    }
}