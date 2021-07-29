using FairPlayTube.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace FairPlayTube.CustomProviders
{
    /// <summary>
    /// Holds the logic to retrieve the current user's information
    /// </summary>
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private const string USER_UNKNOWN = "Unknown";

        private IHttpContextAccessor HttpContextAccessor { get; }
        /// <summary>
        /// Creates a new instance of <see cref="CurrentUserProvider"/>
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the user's username
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            if (this.HttpContextAccessor.HttpContext == null)
            {
                return USER_UNKNOWN;
            }
            else
            {
                var user = this.HttpContextAccessor.HttpContext.User;
                return user?.Identity.Name ?? USER_UNKNOWN;
            }
        }

        /// <summary>
        /// Gets the Logged In User Azure Ad B2C Object Id
        /// </summary>
        /// <returns></returns>
        public string GetObjectId()
        {
            var user = this.HttpContextAccessor.HttpContext.User;
            return user.Claims.Single(p => p.Type == Common.Global.Constants.Claims.ObjectIdentifier).Value;
        }
    }
}
