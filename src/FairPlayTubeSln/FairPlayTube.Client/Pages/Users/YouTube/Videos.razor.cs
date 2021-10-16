using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.UserYouTubeChannel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.YouTube
{
    [Route(Common.Global.Constants.UserYouTubePagesRoutes.Videos)]
    public partial class Videos
    {
        private YouTubeVideoModel[] ChannelVideos;

        [Parameter]
        public long UserId { get; set; }
        [Inject]
        private UserYouTubeChannelClientService UserYouTubeChannelClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private UserYouTubeChannelModel[] UserYouTubeChannels { get; set; }
        private string SelectedChannelId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.UserYouTubeChannels = await this.UserYouTubeChannelClientService
                    .GetUserYouTubeChannelsAsync(UserId);
                if (this.UserYouTubeChannels?.Length > 0)
                {
                    this.SelectedChannelId = this.UserYouTubeChannels[0].YouTubeChannelId;
                    await LoadSelectedChannelVideos();
                }
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }

        public async Task OnChange(ChangeEventArgs e)
        {
            this.SelectedChannelId = e.Value.ToString();
            await LoadSelectedChannelVideos();
        }

        private async Task LoadSelectedChannelVideos()
        {
            try
            {
                this.ChannelVideos = await this.UserYouTubeChannelClientService.GetYouTubeChannelLatestVideos(SelectedChannelId);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
