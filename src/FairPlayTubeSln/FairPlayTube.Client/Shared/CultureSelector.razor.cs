using FairPlayTube.Common.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Shared
{
    public partial class CultureSelector
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IStringLocalizer<CultureSelector> Localizer { get; set; }

        readonly CultureInfo[] cultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("es-CR")
        };

        CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)JSRuntime;
                    js.InvokeVoid("blazorCulture.set", value.Name);

                    NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
                }
            }
        }

        public static string GetDisplayName(CultureInfo culture)
        {
            string result = culture.Name switch
            {
                "en-US" => "English",
                "es-CR" => "Spanish",
                _ => throw new CultureNotFoundException($"Culture '{culture.Name}' not found")
            };
            return result;
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Language")]
        public const string LanguageTextKey = "LanguageText";
        #endregion Resource Keys
    }
}
