using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    public class UploadVideoModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public byte[] FileBytes { get; set; }
    }
}
