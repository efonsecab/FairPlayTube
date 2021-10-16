using FairPlayTube.Common.Global;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Navigation
{
    public static class NavigationHelper
    {
        public static  void NavigateToUserYouTubeVideosPage(NavigationManager navigationManager, long applicationUserId)
        {
            navigationManager.NavigateTo(Constants.UserYouTubePagesRoutes.Videos
                            .Replace("{UserId:long}", applicationUserId.ToString()));
        }
    }
}
