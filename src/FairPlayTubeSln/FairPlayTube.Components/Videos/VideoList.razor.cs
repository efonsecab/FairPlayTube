using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Videos
{
    public partial class VideoList
    {
        [Parameter]
        public VideoInfoModel[] AllVideos { get; set; }
        private VideoInfoModel SelectedVideo { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }

        private void SelectVideo(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
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

        private void HideMonetizationModal()
        {
            this.SelectedVideo = null;
            this.ShowMonetizationLinks = false;
        }
    }
}