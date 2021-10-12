using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Dynamically generates a sitemap
    /// </summary>
    [ApiController]
    public class SitemapController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        /// <summary>
        /// Initializes <see cref="SitemapController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        public SitemapController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        /// <summary>
        /// Generates a sitemap
        /// </summary>
        /// <returns></returns>
        [HttpGet("Sitemap.xml")]
        public void Sitemap()
        {
            
            Response.ContentType = "application/xml";
            StringBuilder stringBuilder = new StringBuilder();
            using var xmlWriter = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true });
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            AddLocation(xmlWriter: xmlWriter, url: "https://fairplaytube.pticostarica.com");
            AddLocation(xmlWriter: xmlWriter, url: "https://fairplaytube.pticostarica.com/Users/Videos/Keywords");
            AddLocation(xmlWriter: xmlWriter, url: "https://fairplaytube.pticostarica.com/Visits");
            try
            {
                var allVideos = FairplaytubeDatabaseContext.VideoInfo.OrderByDescending(p => p.VideoInfoId);
                foreach (var singleVideo in allVideos)
                {
                    AddLocation(xmlWriter: xmlWriter, url: $"https://fairplaytube.pticostarica.com/Public/Videos/Details/{singleVideo.VideoId}");
                }
            }
            catch (Exception)
            {

            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }

        private void AddLocation(XmlWriter xmlWriter, string url)
        {
            xmlWriter.WriteStartElement("url");
            xmlWriter.WriteElementString("loc", url);
            xmlWriter.WriteEndElement();
        }
    }
}
