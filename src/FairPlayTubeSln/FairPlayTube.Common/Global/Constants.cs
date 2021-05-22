using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Global
{
    public class Constants
    {
        public class Claims
        {
            public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
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
        }

        public class ApiRoutes
        {
            public class VideoController
            {
                public const string GetPublicProcessedVideos = "api/Video/GetPublicProcessedVideos";
                public const string UploadVideo = "api/Video/UploadVideo";
                public const string GetMyProcessedVideos = "api/Video/GetMyProcessedVideos";
                public const string GetVideoEditAccessToken = "api/Video/GetVideoEditAccessToken";
            }
            public class UserController
            {
                public const string GetUserRole = "api/User/GetUserRole";
            }
        }
    }
}
