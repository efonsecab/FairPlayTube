using FairPlayTube.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.UploadVideo)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class Upload
    {
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private UploadVideoModel UploadVideoModel = new UploadVideoModel();
        private bool IsLoading { get; set; } = false;
        private bool IsSubmitting { get; set; } = false;

        private async Task OnVideoFileSelectedAsync(InputFileChangeEventArgs e)
        {
            try
            {
                int maxSizeInBytes = 838860800; //800Mbs
                var videoFileStream = e.File.OpenReadStream(maxAllowedSize: maxSizeInBytes);
                this.UploadVideoModel.FileName = e.File.Name;
                MemoryStream memoryStream = new MemoryStream();
                await videoFileStream.CopyToAsync(memoryStream);
                this.UploadVideoModel.FileBytes = memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                this.IsSubmitting = true;
                this.IsLoading = true;
                await this.VideoClientService.UploadVideoAsync(this.UploadVideoModel);
                await this.ToastifyService.DisplaySuccessNotification($"Your video has been uploaded. " +
                    $"It will take some minutes for it to finish being processed");
                this.UploadVideoModel = new UploadVideoModel();
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
                this.IsSubmitting = false;
            }
        }

        private void OnSourceUrlChanged(ChangeEventArgs e)
        {
            string fileName = System.IO.Path.GetFileName(e.Value.ToString());
            this.UploadVideoModel.FileName = fileName;
        }
    }
}
