using FairPlayTube.Common.Interfaces;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Videos
{
    public partial class VideoList
    {
        [Parameter]
        public VideoInfoModel[] AllVideos { get; set; }
        [Parameter]
        public bool AllowEdit { get; set; } = false;
        [Parameter]
        public bool AllowDelete { get; set; } = false;
        [Parameter]
        public EventCallback<VideoInfoModel> OnDelete { get; set; }
        [Inject]
        IVideoEditAccessTokenProvider VideoEditAccessTokenProvider { get; set; }
        private VideoInfoModel SelectedVideo { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }
        private bool ShowVideoDescription { get; set; }

        private async Task SelectVideo(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
            if (AllowEdit)
            {
                this.SelectedVideo.EditAccessToken = await this.VideoEditAccessTokenProvider.GetVideoEditAccessToken(videoInfoModel.VideoId);
            }
            this.ShowInsights = true;
        }

        private void HideInsights()
        {
            this.ShowInsights = false;
            this.SelectedVideo = null;
        }

        private void OnMonetizationIconClicked(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
            this.ShowMonetizationLinks = true;
        }

        private void OnVideoDescriptionClicked(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
            this.ShowVideoDescription = true;
        }

        private void HideMonetizationModal()
        {
            this.SelectedVideo = null;
            this.ShowMonetizationLinks = false;
        }
        private void HideVideoDescriptionModal()
        {
            this.SelectedVideo = null;
            this.ShowVideoDescription = false;
        }

        private string GetVideoInsightsUrl(VideoInfoModel model)
        {
            if (this.AllowEdit)
            {
                return model.PrivateInsightsUrl;
            }
            else
            {
                return model.PublicInsightsUrl;
            }
        }

        private void ShowVideoPlayer(VideoInfoModel videoInfoModel)
        {
            videoInfoModel.ShowPlayerWidget = true;
            StateHasChanged();
        }
   
    }
}