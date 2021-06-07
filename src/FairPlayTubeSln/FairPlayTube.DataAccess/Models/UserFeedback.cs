﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FairPlayTube.DataAccess.Models
{
    public partial class UserFeedback
    {
        [Key]
        public long UserFeedbackId { get; set; }
        [Required]
        [StringLength(500)]
        public string ShortDescription { get; set; }
        [Required]
        public string DetailedDescription { get; set; }
        [Required]
        [StringLength(1000)]
        public string ScreenshotUrl { get; set; }
        public long ApplicationUserId { get; set; }
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

        [ForeignKey(nameof(ApplicationUserId))]
        [InverseProperty("UserFeedback")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}