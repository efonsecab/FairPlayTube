using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FairPlayTube.Common.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetAzureAdB2CUserObjectId(this IEnumerable<Claim> claims)
        {
            return claims.SingleOrDefault(p => p.Type == "oid").Value;
        }

        public static string[] GetUserEmails(this IEnumerable<Claim> claims)
        {
            string emails = claims.FirstOrDefault(p => p.Type == "emails").Value;
            var parsedEmails = System.Text.Json.JsonDocument.Parse(emails);
            string[] emailsArray = parsedEmails.RootElement.EnumerateArray().Select(p => p.GetString()).ToArray();
            return emailsArray;
        }

        public static string GetDisplayName(this IEnumerable<Claim> claims)
        {
            var name = claims.FirstOrDefault(p => p.Type == "name")?.Value;
            return name;
        }
    }
}
