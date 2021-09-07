using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Videos
{
    public partial class CustomProject
    {
        [Parameter]
        public VideoInfoModel[] AvailableVideos { get; set; }
        private ProjectVideoModel SelectedProjectVideoModel { get; set; }
        private ProjectModel ProjectModel { get; set; } = new ProjectModel()
        {
            Videos = new ProjectVideoModel[]
            {
                new ProjectVideoModel()
                {

                }
            }
        };

        private void SelectVideo(VideoInfoModel videoInfoModel)
        {
            this.SelectedProjectVideoModel = new ProjectVideoModel()
            {
                VideoId = videoInfoModel.VideoId
            };
        }
    }
}
