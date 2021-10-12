﻿using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Public.Videos
{
    [Route(Common.Global.Constants.PublicVideosPages.Details)]
    public partial class Details
    {
        [Parameter]
        public string VideoId { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private VideoInfoModel VideoModel { get; set; }
        private bool IsLoading { get; set; }
        private string VideoThumbnailUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string baseUrl = this.NavigationManager.BaseUri;
                var ogThumbnailurl = Constants.ApiRoutes.OpenGraphController.VideoThumbnail.Replace("{videoId}", this.VideoId);
                this.VideoThumbnailUrl = $"{baseUrl}{ogThumbnailurl}";
                IsLoading = true;
                this.VideoModel = await this.VideoClientService.GetVideoAsync(VideoId);
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
    }
}
