using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.ValidateInviteCode)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]

    public partial class ValidateInviteCode
    {
    }
}
