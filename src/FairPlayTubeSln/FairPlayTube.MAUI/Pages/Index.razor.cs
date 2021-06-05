using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MAUI.Pages
{
    public partial class Index
    {
        private VideoInfoModel[] AllPublicVideos { get; private set; }
        [Inject]
        private ClientServices.VideoClientService VideoClientService { get; set; }

        protected override async Task OnInitializedAsync()
            {
            this.AllPublicVideos = await this.VideoClientService.GetPublicProcessedVideosAsync();
        }
    }
}
