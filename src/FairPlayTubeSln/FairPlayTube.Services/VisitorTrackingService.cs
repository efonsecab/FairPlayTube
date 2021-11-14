using FairPlayTube.Common.Providers;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VisitorTracking;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VisitorTrackingService
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private IpStackService IpStackService { get; }

        public VisitorTrackingService(IHttpContextAccessor httpContextAccessor,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext, IpStackService ipStackService)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.IpStackService = ipStackService;
        }

        public async Task<VisitorTracking> TrackVisitAsync(VisitorTrackingModel visitorTrackingModel)
        {
            try
            {
                var httpContext = HttpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (remoteIpAddress == "::1")
                {
                    var ipAddresses = await IpAddressProvider.GetCurrentHostIPv4AddressesAsync();
                    remoteIpAddress = ipAddresses.First();
                }
                var parsedIpAddress = System.Net.IPAddress.Parse(remoteIpAddress);
                var ipGeoLocationInfo = await IpStackService.GetIpGeoLocationInfoAsync(ipAddress: parsedIpAddress);
                string country = ipGeoLocationInfo.country_name;
                var host = httpContext.Request.Host.Value;
                var userAgent = httpContext.Request.Headers["User-Agent"].First();
                ApplicationUser userEntity = null;
                if (!String.IsNullOrWhiteSpace(visitorTrackingModel.UserAzureAdB2cObjectId))
                    userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser.SingleOrDefaultAsync(p => p.AzureAdB2cobjectId.ToString() == visitorTrackingModel.UserAzureAdB2cObjectId);
                var visitedPage = new DataAccess.Models.VisitorTracking()
                {
                    ApplicationUserId = userEntity?.ApplicationUserId,
                    Country = country,
                    Host = host,
                    RemoteIpAddress = remoteIpAddress,
                    UserAgent = userAgent,
                    VisitDateTime = DateTimeOffset.UtcNow,
                    VisitedUrl = visitorTrackingModel.VisitedUrl,
                    SessionId = visitorTrackingModel.SessionId
                };
                await this.FairplaytubeDatabaseContext.VisitorTracking.AddAsync(visitedPage);
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                var pageUri = new Uri(visitedPage.VisitedUrl);
                var lastSegment = pageUri.Segments.Last().TrimEnd('/');
                if (!String.IsNullOrWhiteSpace(lastSegment))
                {
                    var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo.SingleOrDefaultAsync(p => p.VideoId == lastSegment);
                    if (videoInfoEntity != null)
                    {
                        visitedPage.VideoInfoId = videoInfoEntity.VideoInfoId;
                        await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                    }
                }
                return visitedPage;
            }
            catch (Exception ex)
            {
                try
                {
                    await this.FairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                    {
                        FullException = ex.ToString(),
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<VisitorTracking> UpdateVisitTimeElapsedAsync(long visitorTrackingId, CancellationToken cancellationToken)
        {
            var entity = await this.FairplaytubeDatabaseContext.VisitorTracking
                .SingleOrDefaultAsync(p => p.VisitorTrackingId == visitorTrackingId, cancellationToken);
            if (entity != null)
            {
                entity.LastTrackedDateTime = DateTimeOffset.UtcNow;
                await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            }
            return entity;
        }
    }
}
