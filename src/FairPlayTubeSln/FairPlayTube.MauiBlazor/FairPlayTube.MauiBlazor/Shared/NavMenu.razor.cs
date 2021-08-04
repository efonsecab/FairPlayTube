using FairPlayTube.Client.Services;
using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
