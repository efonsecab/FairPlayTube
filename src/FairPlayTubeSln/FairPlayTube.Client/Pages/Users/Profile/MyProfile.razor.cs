using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace FairPlayTube.Client.Pages.Users.Profile
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyProfile)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class MyProfile
    {
    }
}
