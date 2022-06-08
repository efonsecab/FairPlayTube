using FairPlayTube.Client.Navigation;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Public.Videos
{
    [Route(Common.Global.Constants.PublicVideosPages.Details)]
    public partial class Details
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Parameter]
        public string VideoId { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private VideoCommentClientService VideoCommentClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IStringLocalizer<Details> Localizer { get; set; }
        private VideoInfoModel VideoModel { get; set; }
        private VideoCommentModel[] VideoComments { get; set; }
        private bool IsLoading { get; set; }
        private string VideoThumbnailUrl { get; set; }
        private CreateVideoCommentModel NewCommentModel { get; set; } = new CreateVideoCommentModel();
        private bool ShowAddVideoJobButton { get; set; } = false;
        private bool ShowAvailableJobsButton { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private bool ShowAdModal { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (AuthenticationStateTask is not null)
                {
                    var state = await AuthenticationStateTask;
                    if (state is not null && state.User is not null && !state.User.Identity.IsAuthenticated)
                    {
                        this.ShowAdModal = true;
                        StateHasChanged();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        this.ShowAdModal = false;
                        StateHasChanged();
                    }
                    IsLoading = true;
                    ShowAvailableJobsButton = FeatureClientService.IsFeatureEnabled(FeatureType.VideoJobSystem);
                    this.NewCommentModel.VideoId = this.VideoId;
                    string baseUrl = this.NavigationManager.BaseUri;
                    var ogThumbnailurl = Constants.ApiRoutes.OpenGraphController.VideoThumbnail.Replace("{videoId}", this.VideoId);
                    this.VideoThumbnailUrl = $"{baseUrl}{ogThumbnailurl}";
                    this.VideoModel = await this.VideoClientService.GetVideoAsync(VideoId);
                    await LoadComments();
                    if (state is not null && state.User is not null && state.User.Identity.IsAuthenticated)
                    {
                        var myVideos = await VideoClientService.GetMyProcessedVideosAsync();
                        if (myVideos.Any(p => p.VideoId == this.VideoId) &&
                            FeatureClientService.IsFeatureEnabled(FeatureType.VideoJobSystem))
                        {
                            //Logged in user is current video's owner
                            ShowAddVideoJobButton = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JSRuntime.InvokeVoidAsync("prepareWidgets");
            }
        }

        private async Task LoadComments()
        {
            this.VideoComments = await this.VideoCommentClientService.GetVideoCommentsAsync(VideoId);
        }

        private async Task OnValidCommentSubmit()
        {
            try
            {
                this.IsLoading = true;
                await this.VideoCommentClientService.AddVideoCommentAsync(this.NewCommentModel);
                await this.LoadComments();
                this.NewCommentModel.Comment = string.Empty;
            }
            catch (Exception ex)
            {
                this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void OnShowYouTubeLatestVideos(long applicationUserId)
        {
            NavigationHelper.NavigateToUserYouTubeVideosPage(this.NavigationManager, applicationUserId);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Comment")]
        public const string CommentTitleTextKey = "CommentTitleText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitButtonTextKey = "SubmitButtonText";
        [ResourceKey(defaultValue: "Only logged in users are able to add comments")]
        public const string OnlyLoggedInCanAddCommentsTextKey = "OnlyLoggedInCanAddCommentsText";
        [ResourceKey(defaultValue: "Comments")]
        public const string CommentsTitleTextKey = "CommentsTitleText";
        [ResourceKey(defaultValue: "Funded By Ads")]
        public const string FundedByAdsTitleTextKey = "FundedByAdsTitleText";
        [ResourceKey(defaultValue: "This website is funded by ads. To avoid waiting, login into your account")]
        public const string FundedByAdsBodyTextKey = "FundedByAdsBodyText";
        #endregion Resource Keys
    }
}
