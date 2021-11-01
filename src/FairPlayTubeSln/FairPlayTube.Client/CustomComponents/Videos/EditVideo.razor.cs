﻿using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomComponents.Videos
{
    public partial class EditVideo
    {
        [Parameter]
        public VideoInfoModel VideoInfoModel { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<EditVideo> Localizer { get; set; }
        private UpdateVideoModel UpdateVideoModel { get; set; } = new UpdateVideoModel();
        private bool IsSubmitting { get; set; }

        protected override void OnParametersSet()
        {
            this.UpdateVideoModel.Price = this.VideoInfoModel.Price;
        }

        private async Task OnValidSubmit()
        {
            try
            {
                IsSubmitting = true;
                await this.VideoClientService.UpdateMyVideo(this.VideoInfoModel.VideoId, this.UpdateVideoModel);
                await this.ToastifyService.DisplaySuccessNotification("Video has been updated");
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsSubmitting = false;
                StateHasChanged();
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Price")]
        public const string PriceTextKey = "PriceText";
        [ResourceKey(defaultValue: "The Price must be a valid integer number")]
        public const string PriceParsingErrorTextKey = "PriceParsingErrorText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        #endregion Resource Keys
    }
}
