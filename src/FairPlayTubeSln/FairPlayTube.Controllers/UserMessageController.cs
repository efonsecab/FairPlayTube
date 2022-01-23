using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.Users;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data regarding a User Message
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserMessageController: ControllerBase
    {
        private readonly UserMessageService UserMessageService;
        private readonly IMapper Mapper;

        /// <summary>
        /// Initializes <see cref="UserMessageController"/>
        /// </summary>
        /// <param name="userMessageService"></param>
        /// <param name="mapper"></param>
        public UserMessageController(UserMessageService userMessageService,
            IMapper mapper)
        {
            this.UserMessageService=userMessageService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Get all of the users the autheticated user has had conversations with
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize]
        public async Task<ConversationsUserModel[]> GetMyConversationsUsers(CancellationToken cancellationToken)
        {
            var users = await this.UserMessageService.GetMyConversationsUsersAsync(cancellationToken);
            var result = users.Select(p => 
            this.Mapper.Map<ApplicationUser, ConversationsUserModel>(p)).ToArray();
            return result;
        }

        /// <summary>
        /// Retrieves all of the receved messaged from the specified user
        /// </summary>
        /// <param name="otherUserApplicationUserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize]
        public async Task<UserMessageModel[]> GetMyConversationsWithUser(
            long otherUserApplicationUserId, CancellationToken cancellationToken)
        {
            var query = await this.UserMessageService
                .GetMyReceivedMessagesFromUserAsync(otherUserApplicationUserId, cancellationToken);
            var result = await query
                .Select(p => this.Mapper.Map<UserMessage, UserMessageModel>(p))
                .ToArrayAsync(cancellationToken:cancellationToken);
            return result;
        }
    }
}
