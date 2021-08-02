using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Models.FileUpload;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private Language[] AvailableLanguages { get; set; }
        private class Language
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        protected override void OnInitialized()
        {
            var languageList = (new List<Language> {
            new Language() { Name="Chinese (Simplified)", Value="zh-Hans" },
            new Language() { Name="English United Kingdom", Value="en-GB"},
            new Language() { Name="English Australia", Value="en-AU" },
            new Language() { Name="English United States", Value="en-US" },
            new Language() { Name="Spanish", Value="es-ES" },
            new Language() { Name="Spanish (Mexico)", Value="es-MX" },
            new Language() { Name="Finnish", Value="fi-FI" },
            new Language() { Name="French (Canada)", Value="fr-CA" },
            new Language() { Name="French", Value="fr-FR" },
            new Language() { Name="Arabic (Saudi Arabia)", Value="ar-SA" },
            new Language() { Name="Arabic Syrian Arab Republic", Value="ar-SY" },
            new Language() { Name="Arabic (Palestinian Authority)", Value="ar-PS" },
            new Language() { Name="Arabic (Qatar)", Value="ar-QA" },
            new Language() { Name="Arabic Egypt", Value="ar-EG" },
            new Language() { Name="Arabic Modern Standard (Bahrain)", Value="ar-BH" },
            new Language() { Name="Arabic (Oman)", Value="ar-OM" },
            new Language() { Name="Arabic (Lebanon)", Value="ar-LB" },
            new Language() { Name="Arabic (United Arab Emirates)", Value="ar-AE" },
            new Language() { Name="Arabic (Kuwait)", Value="ar-KW" },
            new Language() { Name="Arabic (Jordan)", Value="ar-JO" },
            new Language() { Name="Arabic (Iraq)", Value="ar-IQ" },
            new Language() { Name="Arabic (Israel)", Value="ar-IL" },
            new Language() { Name="Danish", Value="da-DK" },
            new Language() { Name="German", Value="de-DE" },
            new Language() { Name="Czech", Value="cs-CZ" },
            new Language() { Name="Dutch", Value="nl-NL" },
            new Language() { Name="Norwegian", Value="nb-NO" },
            new Language() { Name="Italian", Value="it-IT" },
            new Language() { Name="Japanese", Value="ja-JP" },
            new Language() { Name="Hindi", Value="hi-IN" },
            new Language() { Name="Korean", Value="ko-KR" },
            new Language() { Name="Turkish", Value="tr-TR" },
            new Language() { Name="Thai", Value="th-TH" },
            new Language() { Name="Russian", Value="ru-RU" },
            new Language() { Name="Portuguese", Value="pt-BR" },
            new Language() { Name="Polish", Value="pl-PL" },
            new Language() { Name="Swedish", Value="sv-SE" },
            new Language() { Name="Chinese (Cantonese, Traditional)", Value="zh-HK" }
            }).OrderBy(p => p.Name).ToList();
            languageList.Insert(0, new Language() { Name = "Auto Detect Mult Language", Value = "multi" });
            languageList.Insert(0, new Language() { Name = "Auto Detect Single Language", Value = "auto" });
            AvailableLanguages = languageList.ToArray();
            this.UploadVideoModel.Language = languageList.First().Value;
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

        private string GetVisibilityName(VideoVisibility visibilityValue)
        {
            return Enum.GetName<VideoVisibility>(visibilityValue);
        }

        private void OnFilesUploaded(List<UploadResult> uploadResults)
        {
            var result = uploadResults.Single();
            this.UploadVideoModel.StoredFileName = result.StoredFileName;
        }

        private void OnFileSourceModeChanged(ChangeEventArgs e)
        {
            this.UploadVideoModel.StoredFileName = string.Empty;
            this.UploadVideoModel.SourceUrl = string.Empty;
        }
    }
}
