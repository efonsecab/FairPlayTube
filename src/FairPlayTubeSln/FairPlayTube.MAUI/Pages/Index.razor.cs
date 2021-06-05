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
        private VideoInfoModel[] AllPublicVideos { get; set; }
        [Inject]
        private ClientServices.VideoClientService VideoClientService { get; set; }
        private string ErrorMessage {get;set;}

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.AllPublicVideos = await this.VideoClientService.GetPublicProcessedVideosAsync();
                this.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.ToString();
            }
        }
    }
}
