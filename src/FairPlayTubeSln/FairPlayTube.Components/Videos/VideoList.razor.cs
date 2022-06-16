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
        public bool AllowDownload { get; set; } = false;
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
        [EditorRequired]
        public bool ShowAvailableJobsButton { get; set; }
        private int ItemPos { get; set; } = 0;
   
    }
}