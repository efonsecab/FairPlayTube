using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Global
{
    public class Constants
    {
        public class Claims
        {
            public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        }
        public class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
    }
}
