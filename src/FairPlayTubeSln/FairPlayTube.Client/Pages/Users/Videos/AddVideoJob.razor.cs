using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class AddVideoJob
    {
        [Parameter]
        public VideoInfoModel VideoInfoModel { get; set; }
        [Inject]
        public IStringLocalizer<AddVideoJob> Localizer { get; set; }
        public VideoJobModel VideoJobModel { get; set; } = new VideoJobModel();
        private bool IsLoading { get; set; }

        #region Resource Keys
        [ResourceKey(defaultValue: "Add Video Job")]
        public const string AddVideoJobTitleTextKey = "AddVideoJobTitleText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        #endregion Resource Keys
    }
}
