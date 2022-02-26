using FairPlayTube.Client.CustomLocalization.Api;
using FairPlayTube.Client.Navigation;
using FairPlayTube.Client.Services;
using FairPlayTube.Client.Shared;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Pagination;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages
{
    [Route("/")]
    [Route(Constants.RootPagesRoutes.SearchWithSearchTerm)]
    [Route(Constants.RootPagesRoutes.SearchEmpty)]
    public partial class Index
    {
        [Inject]
        private IStringLocalizer<Index> Localizer { get; set; }

        private PagedItems<VideoInfoModel> PageVideos { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private SearchClientService SearchClientService { get; set; }

        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowAvailableJobsButton { get; set; }
        private bool AllowDownload { get; set; } = false;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter]
        private Error Error { get; set; }
        public string[] AllBoughVideosIds { get; private set; }

        [Parameter]
        public string SearchTerm { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private PageRequestModel CurrentPageRequest = new()
        {
            PageNumber = 1
        };

        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                ShowAvailableJobsButton = FeatureClientService.IsFeatureEnabled(Common.Global.Enums.FeatureType.VideoJobSystem);
                var state = await this.AuthenticationStateTask;
                //if (state.User.Identity.IsAuthenticated)
                //{
                //    AllowDownload = true;
                //}
                PagedItems<VideoInfoModel> pageVideos = null;
                if (String.IsNullOrWhiteSpace(SearchTerm))
                {
                    pageVideos = await this.VideoClientService
                        .GetPublicProcessedVideosAsync(this.CurrentPageRequest);
                }
                else
                {
                    pageVideos = await this.SearchClientService
                        .SearchPublicProcessedVideosAsync(this.CurrentPageRequest, this.SearchTerm);
                }
                if (state.User.Identity.IsAuthenticated)
                {
                    this.AllBoughVideosIds = await this.VideoClientService.GetBoughtVideosIdsAsync();
                    var boughVideos = pageVideos.Items.Where(p => this.AllBoughVideosIds.Contains(p.VideoId));
                    foreach (var singleBoughtVideo in boughVideos)
                    {
                        singleBoughtVideo.IsBought = true;
                    }
                }
                this.PageVideos = pageVideos;
            }
            catch (Exception ex)
            {
                this.ToastifyService.DisplayErrorNotification(ex.Message);
                await this.Error.ProcessErrorAsync(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task OnDownload(VideoInfoModel videoInfoModel)
        {
            try
            {
                var result = await this.VideoClientService.DownloadVideoAsync(videoInfoModel.VideoId);
                await JSRuntime.InvokeVoidAsync(
               "downloadFromByteArray",
               new
               {
                   ByteArray = result.VideoBytes,
                   //infered name
                   videoInfoModel.FileName,
                   ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
               });
            }
            catch (Exception ex)
            {
                this.ToastifyService.DisplayErrorNotification(ex.Message);
                await this.Error.ProcessErrorAsync(ex);
            }
        }

        private async Task OnBuyVideoAccess(VideoInfoModel videoInfoModel)
        {
            try
            {
                await VideoClientService.BuyVideoAccessAsync(videoInfoModel.VideoId);
                ToastifyService.DisplaySuccessNotification("Video Access successfully bought");
                await LoadData();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
                await this.Error.ProcessErrorAsync(ex);
            }
        }

        private void OnShowYouTubeLatestVideos(long applicationUserId)
        {
            NavigationHelper.NavigateToUserYouTubeVideosPage(this.NavigationManager, applicationUserId);
        }

        private async Task LoadPreviousPageAsync()
        {
            try
            {
                this.CurrentPageRequest.PageNumber--;
                await LoadData();
                await JSRuntime.InvokeVoidAsync("scrollToTop");
            }
            catch (Exception ex)
            {
                this.CurrentPageRequest.PageNumber++;
                await this.Error.ProcessErrorAsync(ex);
            }
        }

        private async Task LoadNextPageAsync()
        {
            try
            {
                this.CurrentPageRequest.PageNumber++;
                await LoadData();
                await JSRuntime.InvokeVoidAsync("scrollToTop");
            }
            catch (Exception ex)
            {
                this.CurrentPageRequest.PageNumber--;
                await this.Error.ProcessErrorAsync(ex);
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "What is FairPlayTube?")]
        public const string WhatIsFairPlayTubeTitleKey = "WhatIsFairPlayTube";
        [ResourceKey(defaultValue: "FairPlayTube is the next generation of video sharing portals with a focus on users and transparency.")]
        public const string SystemDescriptionParagraph1Key = "SystemDescriptionParagraph1";
        [ResourceKey(defaultValue: "We are aware of the struggle millions of creators have, " +
            "finding it extremely difficult to earn money they deserve when using other platforms.")]
        public const string StrugglingCreatorsTextKey = "StrugglingCreatorsText";
        [ResourceKey(defaultValue: "At FairPlayTube we have creators in mind, giving them access to monetization options, " +
            "from the very beginning of their journey.")]
        public const string CreatorsInMindTextKey = "CreatorsInMindText";
        [ResourceKey(defaultValue: "We also have content consumers in mind, allowing them to earn money by participating in jobs posted by the creators")]
        public const string ContentConsumersInMindTextKey = "ContentConsumersInMindText";
        [ResourceKey(defaultValue: "Note: FairPlayTube is a young system currently being developed and as such can have bugs." +
            "If you find any, we appreciate your help by adding the information here: ")]
        public const string NotesIssuesTextKey = "NotesIssuesText";
        [ResourceKey(defaultValue: "Report Bug")]
        public const string ReportBugTextKey = "ReportBugText";
        [ResourceKey(defaultValue: "Visit the FairPlayTube Repository on GitHub")]
        public const string VisitRepoTextKey = "VisitRepoText";
        [ResourceKey(defaultValue: "See Known Issues")]
        public const string SeeKnownIssuesTextKey = "SeeKnownIssuesText";
        [ResourceKey(defaultValue: "Welcome To")]
        public const string WelcomeToTextKey = "WelcomeToText";
        [ResourceKey(defaultValue: "About")]
        public const string AboutTextKey = "AboutText";
        #endregion Resource Keys
    }
}
