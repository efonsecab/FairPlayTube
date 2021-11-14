using FairPlayTube.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FairPlayTube.Services
{
    public class RssFeedService
    {
        private VideoService VideoService { get; }

        public RssFeedService(VideoService videoService)
        {
            this.VideoService = videoService;
        }

        public async Task<string> GetPublicProcessedVideosRssAsync(string host)
        {
            var videos = await this.VideoService.GetPublicProcessedVideos()
                .ToListAsync();
            StringWriter stringWriter = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter,
                new XmlWriterSettings() { Async = true, Indent = true }))
            {
                RssFeedWriter rssFeedWriter = new(xmlWriter);
                await rssFeedWriter.WriteTitle("FairPlayTube");
                await rssFeedWriter.WriteDescription("The Next Generation Of Video Sharing Portals focused on users and transparency");
                await rssFeedWriter.WriteGenerator("PTI Costa Rica");
                await rssFeedWriter.WriteValue("link", host);
                foreach (var singleVideo in videos)
                {
                    AtomEntry atomEntry = new()
                    {
                        Title = singleVideo.Name,
                        Description = singleVideo.Description,
                        //Categories = 
                        Id = singleVideo.VideoId,
                        Published = singleVideo.RowCreationDateTime,
                        LastUpdated = singleVideo.RowCreationDateTime,
                        ContentType = "html"
                    };
                    atomEntry.AddLink(
                        new SyndicationLink(new Uri($"{host}/Public/Videos/Details/{singleVideo.VideoId}")));
                    await rssFeedWriter.Write(atomEntry);
                }
            }
            return stringWriter.ToString();
        }
    }
}
