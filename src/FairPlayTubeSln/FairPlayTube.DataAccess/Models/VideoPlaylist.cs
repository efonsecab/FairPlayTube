﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(OwnerApplicationUserId), nameof(PlaylistName), Name = "UI_VideoPlaylist_PlaylistName", IsUnique = true)]
    public partial class VideoPlaylist
    {
        [Key]
        public long VideoPlaylistId { get; set; }
        public long OwnerApplicationUserId { get; set; }
        [Required]
        [StringLength(50)]
        public string PlaylistName { get; set; }
        [Required]
        [StringLength(250)]
        public string PlaylistDescription { get; set; }
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

        [ForeignKey(nameof(OwnerApplicationUserId))]
        [InverseProperty(nameof(ApplicationUser.VideoPlaylist))]
        public virtual ApplicationUser OwnerApplicationUser { get; set; }
    }
}