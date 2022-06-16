using FairPlayTube.Common.Interfaces;
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
        [Parameter]
        public bool ShowAddVideoJobButton { get; set; }
        [Parameter]
        [EditorRequired]
        public bool ShowAvailableJobsButton { get; set; }
        [Inject]
        private IVideoEditAccessTokenProvider VideoEditAccessTokenProvider { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IStringLocalizer<Video> Localizer { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }
        private bool ShowVideoDescription { get; set; }
        private bool ShowDeleteConfirm { get; set; }
        private string EditAccessToken { get; set; }

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
            if (ShowDetailsLink)
            {
                ViewDetails();

            }
            else
            {
                VideoModel.ShowPlayerWidget = true;
                StateHasChanged();
            }
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

        private void NavigateToAddVideoJob()
        {
            string url = Common.Global.Constants.UserPagesRoutes.AddVideoJob.Replace("{VideoId}", this.VideoModel.VideoId);
            NavigationManager.NavigateTo(url);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Duration")]
        public const string VideoDurationTextKey = "VideoDurationText";
        [ResourceKey(defaultValue: "Description")]
        public const string VideoDescriptionTitleKey = "VideoDescriptionTitle";
        [ResourceKey(defaultValue: "Insights")]
        public const string InsightsTextKey = "InsightsText";
        [ResourceKey(defaultValue: "Buy Video Access")]
        public const string BuyVideoAccessTextKey = "BuyVideoAccessText";
        [ResourceKey(defaultValue: "You could earn")]
        public const string YouCouldEarnTextKey = "YouCouldEarnText";
        [ResourceKey(defaultValue: "from")]
        public const string FromTextKey = "FromText";
        [ResourceKey(defaultValue: "Available Job(s)")]
        public const string AvailableJobsTextKey = "AvailableJobsText";
        [ResourceKey(defaultValue: "Watch User YouTube Videos")]
        public const string WatchUserYouTubeVideosTextKey = "WatchUserYouTubeVideosText";
        [ResourceKey(defaultValue: "Watch FairPlayTube Video")]
        public const string WtchFairPlayTubeVideoTextKey = "WtchFairPlayTubeVideoText";
        [ResourceKey(defaultValue: "Delete")]
        public const string DeleteTextKey = "DeleteText";
        [ResourceKey(defaultValue: "Download")]
        public const string DownloadTextKey = "DownloadText";
        [ResourceKey(defaultValue: "Visits")]
        public const string VisitsTextKey = "VisitsText";
        [ResourceKey(defaultValue: "Name")]
        public const string NameTextKey = "NameText";
        [ResourceKey(defaultValue: "Publisher")]
        public const string PublisherTextKey = "PublisherText";
        [ResourceKey(defaultValue: "Price")]
        public const string PriceTextKey = "PriceText";
        [ResourceKey(defaultValue: "Details")]
        public const string DetailsTextKey = "DetailsText";
        [ResourceKey(defaultValue: "Close")]
        public const string CloseTextKey = "CloseText";
        [ResourceKey(defaultValue: "Creators External Monetization")]
        public const string ExternalMonetizationTitleKey = "ExternalMonetizationTitle";
        [ResourceKey(defaultValue: "Say Thanks To Your Creators by Visiting these links")]
        public const string SayThanksTextKey = "SayThanksText";
        [ResourceKey(defaultValue: "One of the main sources if income for content creators is through " +
            "monetization links, such as Affiliate Marketing. " +
            "By visiting these links, you can say thanks to your creators and help them make a living " +
            "to continue creating content for you.")]
        public const string SourceOfIncomeTextKey = "SourceOfIncomeText";
        [ResourceKey(defaultValue: "Video owner does not have global monetization items configured")]
        public const string NoGlobalMonetizationTextKey = "NoGlobalMonetizationText";
        [ResourceKey(defaultValue: "Are you sure you want to delete the selected video")]
        public const string DeleteVideoConfirmationTextKey = "DeleteVideoConfirmationText";
        [ResourceKey(defaultValue: "No")]
        public const string NoTextKey = "NoTextKey";
        [ResourceKey(defaultValue: "Yes")]
        public const string YesTextKey = "YesTextKey";
        #endregion Resource Keys
    }
}
