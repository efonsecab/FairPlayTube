using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Bootstrap
{
    public partial class Loading
    {
        [Parameter]
        public bool IsLoading { get; set; }
    }
}
