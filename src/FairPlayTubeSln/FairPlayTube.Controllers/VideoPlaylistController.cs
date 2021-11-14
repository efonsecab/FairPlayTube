using FairPlayTube.Models.Video;
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
    /// Handles actions related to videos playlists
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoPlaylistController: ControllerBase
    {
        private VideoPlaylistService VideoPlaylistService { get; set; }
        /// <summary>
        /// Initializes <see cref="VideoPlaylistService"/>
        /// </summary>
        /// <param name="videoPlaylistService"></param>
        public VideoPlaylistController(VideoPlaylistService videoPlaylistService)
        {
            this.VideoPlaylistService = videoPlaylistService;
        }


        /// <summary>
        /// Creates a new playlist owned by the logged in user
        /// </summary>
        /// <param name="videoPlaylistModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVideoPlaylist(VideoPlaylistModel videoPlaylistModel, CancellationToken cancellationToken)
        {
            await this.VideoPlaylistService.CreateVideoPlaylisyAsync(videoPlaylistModel, cancellationToken);
            return Ok();
        }


        /// <summary>
        /// Deletes the videoplaylist with the specified id
        /// </summary>
        /// <param name="videoPlaylistId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteVideoPlaylist(long videoPlaylistId, CancellationToken cancellationToken)
        {
            await this.VideoPlaylistService.DeleteVideoPlaylistAsync(videoPlaylistId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoPlaylistItemModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddVideoToPlaylist(VideoPlaylistItemModel videoPlaylistItemModel,
            CancellationToken cancellationToken)
        {
            _ = await VideoPlaylistService.AddVideoToPlaylistAsync(videoPlaylistItemModel, cancellationToken);
            return Ok();
        }


    }
}
