using FairPlayTube.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlayTube.Client.Pages.Admin
{
    [Route(Common.Global.Constants.AdminPagesRoutes.Errors)]
    [Authorize(Roles = Common.Global.Constants.Roles.Admin)]
    public partial class Errors
    {
        [CascadingParameter]
        private Error ErrorComponent { get; set; }
    }
}
