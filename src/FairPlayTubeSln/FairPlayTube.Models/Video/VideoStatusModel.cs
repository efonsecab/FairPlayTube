using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    public class VideoStatusModel
    {
        public string VideoId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ProcessingProgress { get; set; }
    }
}
