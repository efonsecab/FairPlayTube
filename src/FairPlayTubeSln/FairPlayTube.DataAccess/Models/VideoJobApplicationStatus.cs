// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(Name), Name = "UI_VideoJobApplicationStatus_Name", IsUnique = true)]
    public partial class VideoJobApplicationStatus
    {
        public VideoJobApplicationStatus()
        {
            VideoJobApplication = new HashSet<VideoJobApplication>();
        }

        [Key]
        public short VideoJobApplicationStatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(2150)]
        public string Description { get; set; }

        [InverseProperty("VideoJobApplicationStatus")]
        public virtual ICollection<VideoJobApplication> VideoJobApplication { get; set; }
    }
}