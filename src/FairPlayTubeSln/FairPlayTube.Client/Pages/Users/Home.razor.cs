using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.UserHomePage)]
    public partial class Home
    {
        [Parameter]
        public long UserId { get; set; }
    }
}
