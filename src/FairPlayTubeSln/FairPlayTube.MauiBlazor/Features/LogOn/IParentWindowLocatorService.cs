using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Features.LogOn
{
    public interface IParentWindowLocatorService
    {
        object GetCurrentParentWindow();
    }
}
