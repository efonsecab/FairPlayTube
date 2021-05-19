using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Pages.Users
{
    [Route("/Users/Videos")]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class Videos
    {
    }
}
