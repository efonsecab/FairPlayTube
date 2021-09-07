using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.RenderingProject)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class RenderingProject
    {
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        private ToastifyService ToastifyService { get; set; }
        private VideoInfoModel[] AllMyVideos { get; set; }
        private bool IsLoading { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.IsLoading = true;
                this.AllMyVideos = await this.VideoClientService.GetMyProcessedVideos();
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}
