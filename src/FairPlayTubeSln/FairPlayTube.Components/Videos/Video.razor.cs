﻿using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Videos
{
    public partial class Video
    {
        [Parameter]
        public VideoInfoModel VideoModel { get; set; }
        [Parameter]
        public bool AllowEdit { get; set; } = false;
        [Parameter]
        public bool AllowDelete { get; set; } = false;
        [Parameter]
        public bool AllowDownload { get; set; } = false;
        [Parameter]
        public bool ShowDetailsLink { get; set; } = false;
        [Parameter]
        public bool ShowTwitterShareButton { get; set; } = false;
        [Parameter]
        public EventCallback<VideoInfoModel> OnDelete { get; set; }
        [Parameter]
        public EventCallback<VideoInfoModel> OnDownload { get; set; }
        [Parameter]
        public EventCallback<VideoInfoModel> OnBuyVideoAccess { get; set; }
        [Parameter]
        public EventCallback<long> OnShowYouTubeLatestVideos { get; set; }
        [Parameter]
        public bool ShowYouTubeVideosLink { get; set; }
        [Parameter]
        public bool ShowDisplayAd { get; set; }
        [Inject]
        private IVideoEditAccessTokenProvider VideoEditAccessTokenProvider { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        IStringLocalizerFactory LocalizerFactory { get; set; }
        private IStringLocalizer Localizer { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }
        private bool ShowVideoDescription { get; set; }
        private bool ShowDeleteConfirm { get; set; }
        private string EditAccessToken { get; set; }


        protected override void OnInitialized()
        {
            this.Localizer = this.LocalizerFactory.Create(typeof(Video));
        }
        private async Task SelectVideo()
        {
            if (AllowEdit)
            {
                this.EditAccessToken = await this.VideoEditAccessTokenProvider.GetVideoEditAccessToken(VideoModel.VideoId);
                this.VideoModel.EditAccessToken = this.EditAccessToken;
            }
            this.ShowInsights = true;
        }

        private string VideoInsightsUrl
        {
            get
            {
                if (this.AllowEdit)
                {
                    return VideoModel.PrivateInsightsUrl;
                }
                else
                {
                    return VideoModel.PublicInsightsUrl;
                }
            }
        }

        private void HideInsights()
        {
            this.ShowInsights = false;

        }

        private void OnMonetizationIconClicked()
        {
            this.ShowMonetizationLinks = true;
        }

        private void OnVideoDescriptionClicked()
        {
            this.ShowVideoDescription = true;
        }

        private void HideMonetizationModal()
        {
            this.ShowMonetizationLinks = false;
        }
        private void HideVideoDescriptionModal()
        {
            this.ShowVideoDescription = false;
        }

        private void ShowVideoPlayer()
        {
            VideoModel.ShowPlayerWidget = true;
            StateHasChanged();
        }

        private void ViewDetails()
        {
            string formattedUrl = Common.Global.Constants.PublicVideosPages.Details
                .Replace("{VideoId}", VideoModel.VideoId);
            this.NavigationManager.NavigateTo(formattedUrl);
        }

        private async Task OnBuyVideoAccessClicked()
        {
            await this.OnBuyVideoAccess.InvokeAsync(this.VideoModel);
        }

        private void OnDeleteVideoClicked()
        {
            this.ShowDeleteConfirm = true;
        }

        private void OnDeleteVideoCanceled()
        {
            this.ShowDeleteConfirm = false;
        }

        private async Task OnDownloadClicked()
        {
            var downloadTask = OnDownload.InvokeAsync(this.VideoModel);
            await downloadTask;
        }


        private async Task OnDeleteVideoConfirmedAsync()
        {
            var deleteTask = OnDelete.InvokeAsync(this.VideoModel);
            this.ShowDeleteConfirm = false;
            await deleteTask;
        }

        private async Task OnShowYouTubeLatestVideosClicked()
        {
            await this.OnShowYouTubeLatestVideos.InvokeAsync(this.VideoModel.ApplicationUserId);
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Duration")]
        public const string VideoDurationTextKey = "VideoDurationText";
        [ResourceKey(defaultValue:"Description")]
        public const string VideoDescriptionTitleKey = "VideoDescriptionTitle";
        #endregion Resource Keys
    }
}
