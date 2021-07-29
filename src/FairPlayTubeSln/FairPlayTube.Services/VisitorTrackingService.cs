using FairPlayTube.Common.Providers;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VisitorTracking;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Services;
using System;
using System.Linq;
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

        public async Task TrackVisit(VisitorTrackingModel visitorTrackingModel)
        {
            try
            {
                var httpContext = HttpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (remoteIpAddress == "::1")
                {
                    var ipAddresses = IpAddressProvider.GetCurrentHostIPv4Addresses();
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
                await this.FairplaytubeDatabaseContext.VisitorTracking.AddAsync(new DataAccess.Models.VisitorTracking()
                {
                    ApplicationUserId = userEntity != null ? userEntity.ApplicationUserId : null,
                    Country = country,
                    Host = host,
                    RemoteIpAddress = remoteIpAddress,
                    UserAgent = userAgent,
                    VisitDateTime = DateTime.UtcNow,
                    VisitedUrl = visitorTrackingModel.VisitedUrl
                });
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
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
                }
            }
        }
    }
}
