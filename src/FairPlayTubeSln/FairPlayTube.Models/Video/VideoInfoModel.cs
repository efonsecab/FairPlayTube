using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Models.UserProfile;
using System;
using System.Globalization;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Represents the VideoInfo entry
    /// </summary>
    public class VideoInfoModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// Azure Video Indexer Video Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// Name of the Video
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// File's name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Video's Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Azure Video Indexer Video's Location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Azure Video Indexer Video's Account Id
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// Url to use to render the Azure Video Indexer widgets ( not embedded )
        /// </summary>
        public string PublicExternalUrl => $"https://www.videoindexer.ai/accounts/{AccountId}/videos/{VideoId}?locale={CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}&location={Location}&autoplay=false";
        /// <summary>
        /// Url to use to render the Azure Video Indexer Player Widget
        /// </summary>
        public string PublicPlayerUrl => $"https://www.videoindexer.ai/embed/player/{AccountId}/{VideoId}" +
                $"?locale={CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}&location={Location}&autoplay=false";
        /// <summary>
        /// Url to use to render the Azure Video Indexer Insights Widget
        /// </summary>
        public string PublicInsightsUrl => $"https://www.videoindexer.ai/embed/insights/{AccountId}/{VideoId}" +
            $"?locale={CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}&location={Location}&language={CultureInfo.CurrentUICulture.Name}";
        /// <summary>
        /// Access Token required to be able to edit Azure Video Indexer Videos Insights
        /// </summary>
        public string EditAccessToken { get; set; }
        /// <summary>
        /// Url to use to display the insights Widget in editable mode
        /// </summary>
        public string PrivateInsightsUrl => $"{PublicInsightsUrl}&accessToken={EditAccessToken}";
        /// <summary>
        /// Total duration of the video, in seconds
        /// </summary>
        public float VideoDurationInSeconds { get; set; }
        /// <summary>
        /// Video's Duration
        /// </summary>
        public TimeSpan VideoDuration => TimeSpan.FromSeconds(VideoDurationInSeconds);
        /// <summary>
        /// Video's Duration Formatted for Displaying
        /// </summary>
        public string VideoDurationFormatted => VideoDuration.ToString(@"hh\:mm\:ss");
        /// <summary>
        /// Video's Owner
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// Monetization Profile Information
        /// </summary>
        public GlobalMonetizationModel UserGlobalMonetization { get; set; }
        /// <summary>
        /// Video's Price
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// Video's price after commission
        /// </summary>
        public decimal PriceAfterCommission => Price + (Price * Common.Global.Constants.Commissions.VideoAccess);
        /// <summary>
        /// Available Jobs associated with this video
        /// </summary>
        public int AvailableJobs { get; set; }
        /// <summary>
        /// Combined Budget for all jobs associated to this video
        /// </summary>
        public decimal CombinedBudget { get; set; }
        /// <summary>
        /// Language code selected by the user uploading the video
        /// </summary>
        public string VideoLanguageCode { get; set; }
        /// <summary>
        /// Url for the main Thumbnail
        /// </summary>
        public string ThumbnailUrl { get; set; }
        /// <summary>
        /// Tells the UI if the Player Widget should be displayed
        /// </summary>
        public bool ShowPlayerWidget { get; set; } = false;
        /// <summary>
        /// Video's indexing status
        /// </summary>
        public VideoIndexStatus VideoIndexStatus { get; set; }
        /// <summary>
        /// Specified is the logged in user already bought the video
        /// </summary>
        public bool IsBought { get; set; }
        /// <summary>
        /// Number of YouTube channels the user has
        /// </summary>
        public int YouTubeChannels { get; set; }
        /// <summary>
        /// Number of visits the video has had
        /// </summary>
        public long VisitsCount { get; set; }
    }
}
