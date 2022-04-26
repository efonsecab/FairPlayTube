using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.CustomHelpers;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.FileUpload;
using FairPlayTube.Models.UserSubscription;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.UploadVideo)]
    [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
    public partial class Upload
    {
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        UserClientService UserClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<Upload> Localizer { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private UploadVideoModel UploadVideoModel = new();
        private bool IsLoading { get; set; } = false;
        private bool IsSubmitting { get; set; } = false;

        private Language[] AvailableLanguages { get; set; }
        public int VideoNameMaxLength { get; set; }
        public int VideoNameRemainingCharacterCount => VideoNameMaxLength - this.UploadVideoModel?.Name?.Length ?? 0;

        private UserSubscriptionStatusModel MySubscriptionStatus { get; set; }
        private bool IsAllowedToUpload = false;
        private bool HasReachedMaxAllowedWeeklyUploads { get; set; }
        private VideoUploadWizardStage UploadWizardStage { get; set; } = VideoUploadWizardStage.FileNameAndDescriptionInput;
        private bool ShowSubmitButton { get; set; } = true;
        private class Language
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                await LoadSubscriptionStatusAsync();
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
                this.VideoNameMaxLength = Convert.ToInt32(
                    DisplayHelper.MaxLengthFor<UploadVideoModel>(p => p.Name));
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadSubscriptionStatusAsync()
        {
            this.MySubscriptionStatus = await UserClientService.GetMySubscriptionStatusAsync();
            if (this.MySubscriptionStatus != null)
            {
                if (this.MySubscriptionStatus.SubscriptionPlanId == (short)SubscriptionPlan.Unlimited)
                {
                    HasReachedMaxAllowedWeeklyUploads = false;
                    IsAllowedToUpload = true;
                }
                else
                {
                    if (this.MySubscriptionStatus.UploadedVideosLast7Days < this.MySubscriptionStatus.MaxAllowedWeeklyVideos)
                    {
                        HasReachedMaxAllowedWeeklyUploads = false;
                        IsAllowedToUpload = true;
                    }
                    else
                    {
                        HasReachedMaxAllowedWeeklyUploads = true;
                        IsAllowedToUpload = false;
                    }
                }
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                this.IsSubmitting = true;
                this.IsLoading = true;
                switch (this.UploadWizardStage)
                {
                    case VideoUploadWizardStage.FileNameAndDescriptionInput:
                        this.UploadWizardStage = VideoUploadWizardStage.FileSourceMode;
                        break;
                    case VideoUploadWizardStage.FileSourceMode:
                        if (!this.UploadVideoModel.UseSourceUrl)
                            this.ShowSubmitButton = false;
                        this.UploadWizardStage = VideoUploadWizardStage.FileSourceInput;
                        break;
                    case VideoUploadWizardStage.FileSourceInput:
                        this.ShowSubmitButton = true;
                        this.UploadWizardStage = VideoUploadWizardStage.VideoLanguageInput;
                        break;
                    case VideoUploadWizardStage.VideoLanguageInput:
                        this.UploadWizardStage=VideoUploadWizardStage.VideoPriceInput;
                        break;
                    case VideoUploadWizardStage.VideoPriceInput:
                        this.UploadWizardStage = VideoUploadWizardStage.VideoVisibilityInput;
                        break;
                    case VideoUploadWizardStage.VideoVisibilityInput:
                        await this.VideoClientService.UploadVideoAsync(this.UploadVideoModel);
                        this.ToastifyService.DisplaySuccessNotification($"Your video has been uploaded. " +
                            $"It will take some minutes for it to finish being processed");
                        this.NavigationManager.NavigateTo(Constants.UserPagesRoutes.MyPendingVideosStatus);
                        break;
                }
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
                this.IsSubmitting = false;
            }
        }

        private static string GetVisibilityName(VideoVisibility visibilityValue)
        {
            return Enum.GetName<VideoVisibility>(visibilityValue);
        }

        private void OnFilesUploaded(List<UploadResult> uploadResults)
        {
            var result = uploadResults.Single();
            this.UploadVideoModel.StoredFileName = result.StoredFileName;
            this.ShowSubmitButton = true;
        }

        private void OnFileSourceModeChanged(ChangeEventArgs e)
        {
            this.UploadVideoModel.StoredFileName = string.Empty;
            this.UploadVideoModel.SourceUrl = string.Empty;
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Upload")]
        public const string UploadTextKey = "UploadText";
        [ResourceKey(defaultValue: "Name")]
        public const string NameTextKey = "NameText";
        [ResourceKey(defaultValue: "Use Url")]
        public const string UseUrlTextKey = "UseUrlText";
        [ResourceKey(defaultValue: "Source Url")]
        public const string SourceUrlTextKey = "SourceUrlText";
        [ResourceKey(defaultValue: "Video's Language")]
        public const string VideoLanguageTextKey = "VideoLanguageText";
        [ResourceKey(defaultValue: "Description")]
        public const string DescriptionTextKey = "DescriptionText";
        [ResourceKey(defaultValue: "Price")]
        public const string PriceTextKey = "PriceText";
        [ResourceKey(defaultValue: "The Price must be a valid integer number")]
        public const string PriceParsingErrorTextKey = "PriceParsingErrorText";
        [ResourceKey(defaultValue: "Visibility")]
        public const string VisibilityTextKey = "VisibilityText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        [ResourceKey(defaultValue: "Note: Once uploaded, videos will be visible until they finish processing, it could take up to 10 minutes for small videos. " +
            "The longer the video, the longer the processing time will take")]
        public const string NoteTextKey = "NoteText";
        [ResourceKey(defaultValue: "Maximum allowed weekly video uploads reached")]
        public const string MaxAllowedWeeklyVideosReachedTextKey = "MaxAllowedWeeklyVideosReachedText";
        #endregion Resource Keys
    }

    public enum VideoUploadWizardStage
    {
        FileNameAndDescriptionInput = 0,
        FileSourceMode = 1,
        FileSourceInput = 2,
        VideoLanguageInput=3,
        VideoPriceInput = 5,
        VideoVisibilityInput=6
    }
}
