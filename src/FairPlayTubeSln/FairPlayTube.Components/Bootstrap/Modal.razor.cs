using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Bootstrap
{
    public partial class Modal
    {
        [Parameter]
        public RenderFragment Title { get; set; }
        [Parameter]
        public RenderFragment Content { get; set; }
        [Parameter]
        public EventCallback OnCloseButtonClicked { get; set; }
        [Parameter]
        public string CloseButtonText { get; set; }
        [Parameter]
        public EventCallback OnOkButtonClicked { get;set; }
        [Parameter]
        public string OkButtonText { get; set; }
    }
}
