using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Global.Enums.Localizers
{
    public class UserRequestTypeLocalizer
    {
        public static IStringLocalizer<UserRequestTypeLocalizer> Localizer { get; set; }
        public static string ContentRequestDisplayName => Localizer[ContentRequestDisplayNameTextKey];
        public static string FeatureRequestDisplayName => Localizer[FeatureRequestDisplayNameTextKey];

        #region Resource keys
        [ResourceKey(defaultValue:"Content Request")]
        public const string ContentRequestDisplayNameTextKey = "ContentRequestDisplayNameText";
        [ResourceKey(defaultValue: "Feature Request")]
        public const string FeatureRequestDisplayNameTextKey = "FeatureRequestDisplayNameText";
        #endregion Resource keys
    }
}
