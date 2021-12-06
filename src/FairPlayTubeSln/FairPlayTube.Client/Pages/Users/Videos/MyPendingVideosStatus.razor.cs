using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyPendingVideosStatus)]
    [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
    public partial class MyPendingVideosStatus
    {
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<MyPendingVideosStatus> Localizer { get; set; }
        private bool IsLoading { get; set; }
        private List<VideoStatusModel> MyPendingVideosQueue { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.MyPendingVideosQueue = await VideoClientService.GetMyPendingVideosQueueAsync();
            }
            catch (Exception ex)
            {
                this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "My Videos Status")]
        public const string MyPendingVideosStatusTextKey = "MyPendingVideosStatusText";
        [ResourceKey(defaultValue:"Status")]
        public const string StatusTextKey = "StatusText";
        [ResourceKey(defaultValue:"Progress")]
        public const string ProgressTextKey = "ProgressText";
        [ResourceKey(defaultValue: "There are no pending videos on your queue")]
        public const string NoPendingVideosTextKey = "NoPendingVideosText";
        #endregion Resource Keys
    }
}
