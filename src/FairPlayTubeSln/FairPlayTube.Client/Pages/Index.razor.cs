using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        private VideoInfoModel[] AllVideos { get; set; }
        private VideoInfoModel SelectedVideo { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private SearchClientService SearchClientService { get; set; }

        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private bool IsLoading { get; set; }
        private bool AllowDownload { get; set; } = false;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        public string[] AllBoughVideosIds { get; private set; }

        [Parameter]
        public string SearchTerm { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                var state = await this.AuthenticationStateTask;
                //if (state.User.Identity.IsAuthenticated)
                //{
                //    AllowDownload = true;
                //}
                VideoInfoModel[] allVideos = null;
                if (String.IsNullOrWhiteSpace(SearchTerm))
                {
                    allVideos = await this.VideoClientService.GetPublicProcessedVideosAsync();
                }
                else
                {
                    allVideos = await this.SearchClientService.SearchPublicProcessedVideosAsync(this.SearchTerm);
                }
                if (state.User.Identity.IsAuthenticated)
                {
                    this.AllBoughVideosIds = await this.VideoClientService.GetBoughtVideosIdsAsync();
                    var boughVideos = allVideos.Where(p => this.AllBoughVideosIds.Contains(p.VideoId));
                    foreach (var singleBoughtVideo in boughVideos)
                    {
                        singleBoughtVideo.IsBought = true;
                    }
                }
                this.AllVideos = allVideos;
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
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
                var result = await this.VideoClientService.DownloadVideo(videoInfoModel.VideoId);
                await JSRuntime.InvokeVoidAsync(
               "downloadFromByteArray",
               new
               {
                   ByteArray = result.VideoBytes,
                   FileName = videoInfoModel.FileName,
                   ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
               });
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }

        private async Task OnBuyVideoAccess(VideoInfoModel videoInfoModel)
        {
            try
            {
                await VideoClientService.BuyVideoAccessAsync(videoInfoModel.VideoId);
                await ToastifyService.DisplaySuccessNotification("Video Access successfully bought");
                await LoadData();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
