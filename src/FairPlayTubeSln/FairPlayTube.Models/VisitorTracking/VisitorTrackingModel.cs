using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.VisitorTracking
{
    public class VisitorTrackingModel
    {
        [Url]
        public string VisitedUrl { get; set; }
        public string UserAzureAdB2cObjectId { get; set; }
    }
}
