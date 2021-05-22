using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        public UserController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        /// <summary>
        /// Gets the name of the role assigned to the specified user
        /// </summary>
        /// <param name="userAdB2CObjectId"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<string> GetUserRole(Guid userAdB2CObjectId)
        {
            var role = await this.FairplaytubeDatabaseContext.ApplicationUserRole
                .Include(p => p.ApplicationUser)
                .Include(p => p.ApplicationRole)
                .Where(p => p.ApplicationUser.AzureAdB2cobjectId == userAdB2CObjectId)
                .Select(p => p.ApplicationRole.Name).SingleAsync();
            return role;
        }
    }
}
