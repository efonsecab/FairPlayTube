using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Public.Videos
{
    [Route(Common.Global.Constants.PublicVideosPages.VideosJobs)]
    public partial class VideosJobs
    {
        [Inject]
        private VideoJobClientService VideoJobClientService { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<VideosJobs> Localizer { get; set; }
        private bool IsLoading { get; set; }

        private VideoJobModel[] AllVideosJobs { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllVideosJobs = await this.VideoJobClientService.GetVideosJobs();
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<VideoInfoModel> GetVideoAsync(string videoId)
        {
            return await this.VideoClientService.GetVideoAsync(videoId);
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Videos Jobs")]
        public const string VideosJobsTitleTextKey = "VideosJobsTitleText";
        [ResourceKey(defaultValue: "Apply To Video Job")]
        public const string ApplyToVideoJobTextKey = "ApplyToVideoJobText";
        #endregion Resource Keys
    }
}
