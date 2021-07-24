﻿using FairPlayTube.Client.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.Details)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class Details
    {
        [Parameter]
        public string VideoId { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private VideoInfoModel VideoInfoModel { get; set; }
        private bool IsLoading { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.VideoInfoModel = await this.VideoClientService.GetVideoAsync(this.VideoId);
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