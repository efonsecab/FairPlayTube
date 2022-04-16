using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Features.LogOn
{
    public class UserState
    {
        public static UserContext UserContext { get; set; } = new UserContext();
    }
    public class UserContext
    {
        public string Name { get; internal set; }
        public string UserIdentifier { get; internal set; }
        public bool IsLoggedOn { get; internal set; }
        public string GivenName { get; internal set; }
        public string FamilyName { get; internal set; }
        public string Province { get; internal set; }
        public string PostalCode { get; internal set; }
        public string Country { get; internal set; }
        public string EmailAddress { get; internal set; }
        public string JobTitle { get; internal set; }
        public string StreetAddress { get; internal set; }
        public string City { get; internal set; }
        public string AccessToken { get; internal set; }
    }
}
