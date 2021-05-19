using FairPlayTube.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Models.AzureVideoIndexerService.GetAllVideos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Pages
{
    public partial class Index
    {
        public VideoInfo[] AllVideos { get; private set; }
        [Inject]
        private VideoService VideoService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private AzureVideoIndexerConfiguration AzureVideoIndexerConfiguration { get; set; }
        private bool IsLoading { get; set; }
        private string ErrorMessage { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllVideos = await this.VideoService.GetPublicProcessedVideosAsync();
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!String.IsNullOrWhiteSpace(this.ErrorMessage))
            {
                await this.ToastifyService.DisplayErrorNotification(this.ErrorMessage);
            }
        }

        private string GetVideoPlayerUrl(VideoInfo videoInfo)
        {
            return $"https://www.videoindexer.ai/embed/player/{videoInfo.accountId}/{videoInfo.id}/" +
                $"?&locale=en&location={this.AzureVideoIndexerConfiguration.Location}";
        }
    }
}
