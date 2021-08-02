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

namespace FairPlayTube.Client.CustomComponents.Upload
{
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class FileUpload
    {
        [Inject]
        HttpClientService HttpClientService { get; set; }
        [Inject]
        ILogger<FileUpload> Logger { get; set; }
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
                long maxFileSize = 1024 * 1024 * 15;
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
                    var response = await authorizedHttpClient.PostAsync("api/Filesave/PostFile", content);

                    var newUploadResults = await response.Content
                        .ReadFromJsonAsync<IList<UploadResult>>();

                    uploadResults = uploadResults.Concat(newUploadResults).ToList();
                    await this.OnFilesUploaded.InvokeAsync(uploadResults);
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
