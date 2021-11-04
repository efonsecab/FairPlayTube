using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Global.Enums
{
    public enum VideoJobApplicationStatus
    {
        New = 1,
        Selected = 2,
        NotSelected = 3,
        PendingPayment=4,
        Paid=5
    }
}
