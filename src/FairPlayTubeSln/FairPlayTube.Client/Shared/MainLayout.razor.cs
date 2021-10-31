using FairPlayTube.Common.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private IStringLocalizer<MainLayout> Localizer { get; set; }
        #region Resource Keys
        [ResourceKey(defaultValue: "Note: Some browser extensions block you from playing videos. " +
            "If all you see is a blank screen, you will need to disable them")]
        public const string BrowsersExensionsWarningTextKey = "BrowsersExensionsWarningText";
        [ResourceKey(defaultValue:"Please fill")]
        public const string PleaseFillTextKey = "PleaseFillText";
        [ResourceKey(defaultValue:"This survey")]
        public const string ThisSurveyTextKey = "ThisSurveyText";
        [ResourceKey(defaultValue:"Designed By")]
        public const string DesignedByTextKey = "DesignedByText";
        [ResourceKey(defaultValue: "Request Demo")]
        public const string RequestDemoTextKey = "RequestDemoText";
        #endregion Resource Keys
    }
}
