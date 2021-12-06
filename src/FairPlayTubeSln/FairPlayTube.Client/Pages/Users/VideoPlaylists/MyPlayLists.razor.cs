using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.VideoPlaylists
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyPlaylists)]
    [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
    public partial class MyPlayLists
    {
        [Inject]
        private IStringLocalizer<MyPlayLists> Localizer { get; set; }
        [Inject]
        private VideoPlaylistClientService VideoPlaylistClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private readonly VideoPlaylistModel Model = new();
        private bool IsLoading { get; set; }

        private async Task OnValidSubmittedPlaylist()
        {
            try
            {
                IsLoading = true;
                await this.VideoPlaylistClientService.CreateVideoPlaylistAsync(this.Model);
                ToastifyService.DisplaySuccessNotification(Localizer[NewPlaylistCreatedTextKey]);
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

        #region Resource Keys
        [ResourceKey(defaultValue:"My Playlists")]
        public const string MyPlaylistsTitleKey = "MyPlaylistsTitle";
        [ResourceKey(defaultValue:"Playlist Name")]
        public const string PlaylistNameTextKey = "PlaylistNameText";
        [ResourceKey(defaultValue: "Playlist Description")]
        public const string PlaylistDescriptionTextKey = "PlaylistDescriptionText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        [ResourceKey(defaultValue: "Your new video playlist has been created")]
        public const string NewPlaylistCreatedTextKey = "NewPlaylistCreatedText";
        #endregion Resource Keys

    }
}
