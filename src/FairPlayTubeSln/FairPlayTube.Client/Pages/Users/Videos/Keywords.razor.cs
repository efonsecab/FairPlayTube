using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.Keywords)]
    public partial class Keywords
    {
        private bool IsLoading { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        private GlobalKeywordModel[] AllKeywords { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private VideoInfoModel[] FoundVideos { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllKeywords = await this.VideoClientService.ListAllKeywordsAsync();
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

        private async Task OnKeywordSearchChanged(ChangeEventArgs e)
        {
            if (e.Value == null)
                return;
            string searchTerm = e.Value.ToString();
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                this.FoundVideos = null;
            }
            else
            {
                this.FoundVideos = await this.VideoClientService.ListVideosByKeywordAsync(Keyword: searchTerm);
            }
        }
    }
}
