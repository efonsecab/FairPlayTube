using FairPlayTube.Client.Navigation;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Public.Videos
{
    [Route(Common.Global.Constants.PublicVideosPages.Details)]
    public partial class Details
    {
        [Parameter]
        public string VideoId { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private VideoCommentClientService VideoCommentClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private VideoInfoModel VideoModel { get; set; }
        private VideoCommentModel[] VideoComments { get; set; }
        private bool IsLoading { get; set; }
        private string VideoThumbnailUrl { get; set; }
        private CreateVideoCommentModel NewCommentModel { get; set; } = new CreateVideoCommentModel();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.NewCommentModel.VideoId = this.VideoId;
                string baseUrl = this.NavigationManager.BaseUri;
                var ogThumbnailurl = Constants.ApiRoutes.OpenGraphController.VideoThumbnail.Replace("{videoId}", this.VideoId);
                this.VideoThumbnailUrl = $"{baseUrl}{ogThumbnailurl}";
                IsLoading = true;
                this.VideoModel = await this.VideoClientService.GetVideoAsync(VideoId);
                await LoadComments();
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

        private async Task LoadComments()
        {
            this.VideoComments = await this.VideoCommentClientService.GetVideoCommentsAsync(VideoId);
        }

        private async Task OnValidCommentSubmit()
        {
            try
            {
                this.IsLoading = true;
                await this.VideoCommentClientService.AddVideoCommentAsync(this.NewCommentModel);
                await this.LoadComments();
                this.NewCommentModel.Comment = string.Empty;
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void OnShowYouTubeLatestVideos(long applicationUserId)
        {
            NavigationHelper.NavigateToUserYouTubeVideosPage(this.NavigationManager, applicationUserId);
        }

        private MarkupString GetFormattedComment(VideoCommentModel singleComment)
        {
            //based on sample here: https://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string replacedText = singleComment.Comment;
            foreach (Match m in linkParser.Matches(singleComment.Comment))
            {
                replacedText = replacedText.Replace(m.Value, $"<a href=\"{m.Value}\" target=\"_blank\">{m.Value}</a>");
            }
            return (MarkupString)replacedText;
        }
    }
}
