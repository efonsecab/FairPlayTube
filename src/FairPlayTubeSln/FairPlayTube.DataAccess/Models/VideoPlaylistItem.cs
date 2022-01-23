﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(VideoPlaylistId), nameof(VideoInfoId), Name = "UI_VideoPlaylistItem_Video", IsUnique = true)]
    [Index(nameof(VideoInfoId), nameof(Order), Name = "UI_VideoPlaylistItem_VideoOrder", IsUnique = true)]
    public partial class VideoPlaylistItem
    {
        [Key]
        public long VideoPlaylistItemId { get; set; }
        public long VideoPlaylistId { get; set; }
        public long? VideoInfoId { get; set; }
        public int Order { get; set; }
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

        [ForeignKey(nameof(VideoInfoId))]
        [InverseProperty("VideoPlaylistItem")]
        public virtual VideoInfo VideoInfo { get; set; }
        [ForeignKey(nameof(VideoPlaylistId))]
        [InverseProperty("VideoPlaylistItem")]
        public virtual VideoPlaylist VideoPlaylist { get; set; }
    }
}