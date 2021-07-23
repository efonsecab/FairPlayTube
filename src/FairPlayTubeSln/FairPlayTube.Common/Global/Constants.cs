using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Global
{
    public class Constants
    {
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

        public class UserPagesRoutes
        {
            public const string UploadVideo = "/Users/Videos/Upload";
            public const string MyVideos = "/Users/Videos/MyVideos";
            public const string Keywords = "/Users/Videos/Keywords";
            public const string ProfileMonetization = "/Users/Profile/Monetization";
            public const string List = "/Users/List";
            public const string MyFunds = "/Users/MyFunds";
        }

        public class ApiRoutes
        {
            public class VideoController
            {
                public const string GetPublicProcessedVideos = "api/Video/GetPublicProcessedVideos";
                public const string ListVideosByKeyword = "api/Video/ListVideosByKeyword";
                public const string UploadVideo = "api/Video/UploadVideo";
                public const string GetMyProcessedVideos = "api/Video/GetMyProcessedVideos";
                public const string GetVideoEditAccessToken = "api/Video/GetVideoEditAccessToken";
                public const string ListAllKeywords = "api/Video/ListAllKeywords";
            }
            public class UserController
            {
                public const string GetMyRole = "api/User/GetMyRole";
                public const string ListUsers = "api/User/ListUsers";
            }

            public class UserProfileController
            {
                public const string SaveMonetization = "api/UserProfile/SaveMonetization";
                public const string GetMyMonetizationInfo = "api/UserProfile/GetMyMonetizationInfo";
            }
        }
    }
}
