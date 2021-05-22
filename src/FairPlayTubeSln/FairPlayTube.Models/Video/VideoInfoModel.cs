using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    public class VideoInfoModel
    {
        public string VideoId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AccountId { get; set; }
        public string PublicPlayerUrl => $"https://www.videoindexer.ai/embed/player/{AccountId}/{VideoId}" +
                $"?&locale=en&location={Location}";
        public string PublicInsightsUrl => $"https://www.videoindexer.ai/embed/insights/{AccountId}/{VideoId}" +
            $"/?&locale=en&location={Location}";
        public string EditAccessToken { get; set; }
        public string PrivateInsightsUrl => $"{PublicInsightsUrl}&accessToken={EditAccessToken}";
    }
}
