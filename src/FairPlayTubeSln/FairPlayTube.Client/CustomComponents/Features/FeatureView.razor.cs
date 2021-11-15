using FairPlayTube.Common.Global.Enums;
using Microsoft.AspNetCore.Components;

namespace FairPlayTube.Client.CustomComponents.Features
{
    public partial class FeatureView
    {
        [Parameter]
        public RenderFragment FeatureEnabled { get; set; }
        [Parameter]
        public RenderFragment FeatureDisabled { get; set; }
        [Parameter]
        [EditorRequired]
        public FeatureType FeatureType { get; set; }
    }
}
