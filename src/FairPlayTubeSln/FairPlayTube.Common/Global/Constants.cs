﻿namespace FairPlayTube.Common.Global
{
    public class Constants
    {
        public class RegularExpressions
        {
            public const string AllowedFileNameFormat = "^[a-zA-Z0-9 ]+$";
        }
        public class UploadLimits
        {
            public const int MaxMegabytesAllowed = 300;
            public const long MaxBytesAllowed = 1024 * 1024 * MaxMegabytesAllowed;
        }
        public class Commissions
        {
            public const decimal VideoAccess = 0.05M;//5% 
        }
        public class PriceLimits
        {
            public const int MinVideoPrice = 0;
            public const int MaxVideoPrice = 100;
        }
        public class CurrencySymbols
        {
            public const string Dollars = "$";
        }
        public class Hubs
        {
            public const string NotificationHub = "/NotificationHub";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessage";
        }

        public class ConfigurationKeysNames
        {
            public const string AzureAppConfigConnectionString = "AzureAppConfigConnectionString";
            public const string DefaultConnectionString = "Default";
        }
        public class Titles
        {
            public const string AppTitle = "FairPlayTube";
        }
        public class Claims
        {
            public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            public const string Name = "name";
            public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
            public const string SurName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
            public const string Emails = "emails";
        }
        public class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }

        public class PublicVideosPages
        {
            public const string Details = "/Public/Videos/Details/{VideoId}";
        }

        public class UserYouTubePagesRoutes
        {
            public const string Videos = "/Users/{UserId:long}/YouTube/Videos";
        }
        public class UserPagesRoutes
        {
            public const string UploadVideo = "/Users/Videos/Upload";
            public const string MyVideos = "/Users/Videos/MyVideos";
            public const string Keywords = "/Users/Videos/Keywords";
            public const string ProfileMonetization = "/Users/Profile/Monetization";
            public const string List = "/Users/List";
            public const string MyFunds = "/Users/MyFunds";
            public const string Edit = "/Users/Videos/Edit/{VideoId}";
            public const string MyPendingVideosStatus = "/Users/Videos/MyPendingVideosStatus";
            public const string UserHomePage = "/Users/Home/{UserId:long}";
            public const string ValidateInviteCode = "/Users/ValidateInviteCode";
            public const string InviteUser = "/Users/InviteUser";
        }

        public class RootPagesRoutes
        {
            public const string Persons = "/Persons";
            public const string SearchWithSearchTerm = "/Search/{SearchTerm}";
            public const string SearchEmpty = "/Search";
        }

        public class ApiRoutes
        {
            public class VideoCommentController
            {
                public const string GetVideoComments = "api/VideoComment/GetVideoComments";
                public const string AddVideoComment = "api/VideoComment/AddVideoComment";
            }
            public class OpenGraphController
            {
                public const string VideoThumbnail = "api/OpenGraph/VideoThumbnail/{videoId}";
            }
            public class VisitorTrackingController
            {
                public const string TrackAnonymousClientInformation = "api/VisitorTracking/TrackAnonymousClientInformation";
                public const string TrackAuthenticatedClientInformation = "api/VisitorTracking/TrackAuthenticatedClientInformation";
                public const string UpdateVisitTimeElapsed = "api/VisitorTracking/UpdateVisitTimeElapsed";
            }

            public class VideoPlaylistController
            {
                public const string CreateVideoPlaylist = "api/VideoPlaylist/CreateVideoPlaylist";
                public const string DeleteVideoPlaylist = "api/VideoPlaylist/DeleteVideoPlaylist";
                public const string AddVideoToPlaylist = "api/VideoPlaylist/AddVideoToPlaylist";
            }

            public class SearchController
            {
                public const string SearchPublicProcessedVideos = "api/Search/SearchPublicProcessedVideos";
            }

            public class LocalizationController
            {
                public const string GetAllResources = "api/Localization/GetAllResources";
            }
            public class VideoController
            {
                public const string GetPublicProcessedVideos = "api/Video/GetPublicProcessedVideos";
                public const string ListVideosByKeyword = "api/Video/ListVideosByKeyword";
                public const string UploadVideo = "api/Video/UploadVideo";
                public const string GetMyProcessedVideos = "api/Video/GetMyProcessedVideos";
                public const string GetVideoEditAccessToken = "api/Video/GetVideoEditAccessToken";
                public const string ListAllKeywords = "api/Video/ListAllKeywords";
                public const string UpdateMyVideo = "api/Video/UpdateMyVideo";
                public const string GetVideo = "api/Video/GetVideo";
                public const string BuyVideoAccess = "/api/Video/BuyVideoAccess";
                public const string GetMyPendingVideosQueue = "api/Video/GetMyPendingVideosQueue";
                public const string GetPersons = "api/Video/GetPersons";
                public const string DeleteVideo = "api/Video/DeleteVideo";
                public const string DownloadVideo = "api/Video/DownloadVideo";
                public const string GetBoughtVideosIds = "api/Video/GetBoughtVideosIds";
            }
            public class UserController
            {
                public const string GetMyRole = "api/User/GetMyRole";
                public const string ListUsers = "api/User/ListUsers";
                public const string InviteUser = "api/User/InviteUser";
                public const string AddUserFollower = "API/uSER/AddUserFollower";
                public const string SendMessage = "api/User/SendMessage";
                public const string GetMyUserStatus = "api/User/GetMyUserStatus";
                public const string ValidateUserInviteCode = "api/User/ValidateUserInviteCode";
            }

            public class UserProfileController
            {
                public const string SaveMonetization = "api/UserProfile/SaveMonetization";
                public const string GetMyMonetizationInfo = "api/UserProfile/GetMyMonetizationInfo";
                public const string AddFunds = "api/UserProfile/AddFunds";
                public const string GetMyFunds = "api/UserProfile/GetMyFunds";
            }

            public class UserYouTubeChannelController
            {
                public const string GetUserYouTubeChannels = "api/UserYouTubeChannel/GetUserYouTubeChannels";
                public const string GetYouTubeChannelLatestVideos = "api/UserYouTubeChannel/GetYouTubeChannelLatestVideos";
            }
        }
    }
}
