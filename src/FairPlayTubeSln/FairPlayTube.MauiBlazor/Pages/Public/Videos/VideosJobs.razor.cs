using Blazored.Toast.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoJobApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace FairPlayTube.MauiBlazor.Pages.Public.Videos
{
    [Route(Common.Global.Constants.PublicVideosPages.VideosJobs)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class VideosJobs
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        private VideoJobClientService VideoJobClientService { get; set; }
        [Inject]
        private IToastService ToastService { get; set; }
        [Inject]
        private IStringLocalizer<VideosJobs> Localizer { get; set; }
        [Inject]
        private VideoJobApplicationClientService VideoJobApplicationClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private bool IsLoading { get; set; }
        private VideoJobModel[] AvailableVideosJobs { get; set; }
        private CreateVideoJobApplicationModel CreateVideoJobApplicationModel { get; set; } =
            new CreateVideoJobApplicationModel();
        private bool ShowApplyToVideoJobModal { get; set; } = false;
        private VideoJobModel SelectdVideoJob { get; set; }
        private EditForm VideoJobApplicationEditForm { get; set; }
        private VideoJobApplicationModel[] MyVideoJobsApplications { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!FeatureClientService.IsFeatureEnabled(Common.Global.Enums.FeatureType.VideoJobSystem))
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            try
            {
                IsLoading = true;
                await LoadJobs();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadJobs()
        {
            this.AvailableVideosJobs = await this.VideoJobClientService.GetAvailableVideosJobsAsync();
            var state = await AuthenticationStateTask;
            if (state is not null && state.User.Identity.IsAuthenticated)
            {
                this.MyVideoJobsApplications =
                    await this.VideoJobApplicationClientService.GetMyVideoJobsApplicationsAsync();
            }
        }

        private async Task OnApplyToVideoJobClickedAsync(VideoJobModel videoJobModel)
        {
            var state = await AuthenticationStateTask;
            if (state != null && state.User.Identity.IsAuthenticated)
            {
                this.SelectdVideoJob = videoJobModel;
                this.CreateVideoJobApplicationModel.VideoJobId = videoJobModel.VideoJobId;
                this.ShowApplyToVideoJobModal = true;
            }
            else
            {
                ToastService.ShowError(Localizer[MustBeLoggedInTextKey],
                    Localizer[AccessDeniedTextKey]);
            }
        }

        private void CancelVideoJobApplication()
        {
            CleanVideoJobApplication();
        }

        private void CleanVideoJobApplication()
        {
            this.ShowApplyToVideoJobModal = false;
            this.CreateVideoJobApplicationModel.VideoJobId = null;
            this.CreateVideoJobApplicationModel.ApplicantCoverLetter = string.Empty;
        }

        private async Task ApplyToVideoJob()
        {
            if (this.VideoJobApplicationEditForm.EditContext.Validate())
            {
                try
                {
                    IsLoading = true;
                    await this.VideoJobApplicationClientService
                        .AddVideoJobApplicationAsync(this.CreateVideoJobApplicationModel);
                    CleanVideoJobApplication();
                    await LoadJobs();
                    ToastService.ShowSuccess(Localizer[VideoJobApplicationSentTextKey]);
                }
                catch (Exception ex)
                {
                    CleanVideoJobApplication();
                    ToastService.ShowError(ex.Message);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Videos Jobs")]
        public const string VideosJobsTitleTextKey = "VideosJobsTitleText";
        [ResourceKey(defaultValue: "Apply To Video Job")]
        public const string ApplyToVideoJobTextKey = "ApplyToVideoJobText";
        [ResourceKey(defaultValue: "You must be logged in in order to apply to Video Jobs")]
        public const string MustBeLoggedInTextKey = "MustBeLoggedInText";
        [ResourceKey(defaultValue: "Access Denied")]
        public const string AccessDeniedTextKey = "AccessDeniedText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        [ResourceKey(defaultValue: "Your application has been sent")]
        public const string VideoJobApplicationSentTextKey = "VideoJobApplicationSentText";
        #endregion Resource Keys
    }
}
