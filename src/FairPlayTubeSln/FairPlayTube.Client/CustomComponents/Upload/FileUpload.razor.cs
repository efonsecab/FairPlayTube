using FairPlayTube.Models.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using FairPlayTube.ClientServices;
using Microsoft.AspNetCore.Authorization;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.CustomHttpResponse;

namespace FairPlayTube.Client.CustomComponents.Upload
{
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class FileUpload
    {
        [Inject]
        private HttpClientService HttpClientService { get; set; }
        [Inject]
        private ILogger<FileUpload> Logger { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private List<File> files = new();
        private List<UploadResult> uploadResults = new();
        private int maxAllowedFiles = 1;
        [Parameter]
        public EventCallback<List<UploadResult>> OnFilesUploaded { get; set; }
        private bool IsLoading { get; set; }
        private bool ShouldDisplayFileList { get; set; } = false;

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            try
            {
                IsLoading = true;
                ShouldDisplayFileList = false;
                StateHasChanged();
                long maxFileSize = Common.Global.Constants.UploadLimits.MaxBytesAllowed;
                var upload = false;

                using var content = new MultipartFormDataContent();

                foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
                {
                    if (uploadResults.SingleOrDefault(
                        f => f.FileName == file.Name) is null)
                    {
                        try
                        {
                            var fileContent =
                                new StreamContent(file.OpenReadStream(maxFileSize));

                            files.Add(new() { Name = file.Name });

                            content.Add(
                                content: fileContent,
                                name: "\"files\"",
                                fileName: file.Name);

                            upload = true;
                        }
                        catch (Exception ex)
                        {
                            await ToastifyService.DisplayErrorNotification($"Please specify a smaller file. Max: " +
                                $"{Common.Global.Constants.UploadLimits.MaxBytesAllowed / 1024 / 1024} MB");
                            Logger.LogInformation(
                                "{FileName} not uploaded (Err: 6): {Message}",
                                file.Name, ex.Message);

                            uploadResults.Add(
                                new()
                                {
                                    FileName = file.Name,
                                    ErrorCode = 6,
                                    Uploaded = false
                                });
                        }
                    }
                }

                if (upload)
                {
                    var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
                    authorizedHttpClient.Timeout = TimeSpan.FromMinutes(30);
                    var response = await authorizedHttpClient.PostAsync("api/Filesave/PostFile", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        await ToastifyService.DisplayErrorNotification("Unable to upload file. Please try a small file");
                    }
                    else
                    {
                        var newUploadResults = await response.Content
                            .ReadFromJsonAsync<IList<UploadResult>>();

                        uploadResults = uploadResults.Concat(newUploadResults).ToList();
                        await this.OnFilesUploaded.InvokeAsync(uploadResults);
                    }
                }

                ShouldDisplayFileList = true;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private static bool Upload(IList<UploadResult> uploadResults,
            string fileName, ILogger<FileUpload> logger, out UploadResult result)
        {
            result = uploadResults.SingleOrDefault(f => f.FileName == fileName);

            if (result is null)
            {
                logger.LogInformation("{FileName} not uploaded (Err: 5)", fileName);
                result = new();
                result.ErrorCode = 5;
            }

            return result.Uploaded;
        }

        private class File
        {
            public string Name { get; set; }
        }
    }
}
