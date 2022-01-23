﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    public partial class VideoComment
    {
        [Key]
        public long VideoCommentId { get; set; }
        public long VideoInfoId { get; set; }
        public long ApplicationUserId { get; set; }
        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
        public DateTimeOffset RowCreationDateTime { get; set; }
        [StringLength(256)]
        public string RowCreationUser { get; set; }
        [StringLength(250)]
        public string SourceApplication { get; set; }
        [Column("OriginatorIPAddress")]
        [StringLength(100)]
        public string OriginatorIpaddress { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [InverseProperty("VideoComment")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(VideoInfoId))]
        [InverseProperty("VideoComment")]
        public virtual VideoInfo VideoInfo { get; set; }
        [InverseProperty("VideoComment")]
        public virtual VideoCommentAnalysis VideoCommentAnalysis { get; set; }
    }
}