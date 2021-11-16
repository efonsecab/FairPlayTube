using FairPlayTube.Common.Localization;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace FairPlayTube.Client.Pages.Users.Profile
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyProfile)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class MyProfile
    {
        [Inject]
        private IStringLocalizer<MyProfile> Localizer { get; set; }
        private UpdateUserProfileModel UpdateUserProfileModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private static void OnValidSubmit()
        {

        }

        #region Resource Keys
        [ResourceKey(defaultValue: "My Profile")]
        public const string MyProfileTitleTextKey = "MyProfileTitleText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        #endregion Resource Keys
    }
}
