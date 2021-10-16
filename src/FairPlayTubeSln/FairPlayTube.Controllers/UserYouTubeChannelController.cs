using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.UserYouTubeChannel;
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
    /// Handles all of the dat regarding users YouTube channels
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserYouTubeChannelController : ControllerBase
    {
        private UserYouTubeChannelService UserYouTubeChannelService;
        private IMapper Mapper;

        /// <summary>
        /// Initializes <see cref="UserYouTubeChannelController"/>
        /// </summary>
        public UserYouTubeChannelController(UserYouTubeChannelService userYouTubeChannelService,
            IMapper mapper)
        {
            this.UserYouTubeChannelService = userYouTubeChannelService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Add a new youtube channel for a given user
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<UserYouTubeChannelModel> AddUserYouTubeChannel(UserYouTubeChannelModel userYouTubeChannelModel,
            CancellationToken cancellationToken)
        {
            var entity = await this.UserYouTubeChannelService.AddUserYouTubeChannel(userYouTubeChannelModel, cancellationToken);
            return this.Mapper.Map<UserYouTubeChannel, UserYouTubeChannelModel>(entity);
        }

        /// <summary>
        /// Retrieves the Youtube channels infor for a given user
        /// </summary>
        /// <param name="applicationUserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<UserYouTubeChannelModel[]> GetUserYouTubeChannels(long applicationUserId, CancellationToken cancellationToken)
        {
            var result = await this.UserYouTubeChannelService.GetUserYouTubeChannels(applicationUserId)
                .Select(p => this.Mapper.Map<UserYouTubeChannel, UserYouTubeChannelModel>(p))
                .ToArrayAsync();
            return result;
        }
    }
}
