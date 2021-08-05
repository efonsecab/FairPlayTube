using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.MetaTags
{
    public partial class MetaTag
    {
        [Parameter]
        public string TagName { get; set; }
        [Parameter]
        public string TagValue { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await JSRuntime.InvokeVoidAsync("appendMetaTag", TagName, TagValue);
        }
    }
}
