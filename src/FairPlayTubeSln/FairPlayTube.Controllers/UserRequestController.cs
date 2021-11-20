using FairPlayTube.Models.UsersRequests;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data regarding User Requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserRequestController : ControllerBase
    {
        private UserRequestService UserRequestService { get; set; }

        /// <summary>
        /// Instantiates <see cref="UserRequestController"/>
        /// </summary>
        /// <param name="userRequestService"></param>
        public UserRequestController(UserRequestService userRequestService)
        {
            UserRequestService = userRequestService;
        }

        /// <summary>
        /// Adds information for User Requests from non-logged in users
        /// </summary>
        /// <param name="createUserRequestModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAnonymousUserRequestAsync(CreateUserRequestModel createUserRequestModel,
                    CancellationToken cancellationToken)
        {
            await this.UserRequestService.AddAnonymousUserRequestAsync(createUserRequestModel, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Adds information for User Requests from non-logged in users
        /// </summary>
        /// <param name="createUserRequestModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddAuthenticatedUserRequestAsync(CreateUserRequestModel createUserRequestModel,
                    CancellationToken cancellationToken)
        {
            await this.UserRequestService.AddAuthenticatedUserRequestAsync(createUserRequestModel, cancellationToken);
            return Ok();
        }
    }
}
