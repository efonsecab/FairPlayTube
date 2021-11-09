using FairPlayTube.ClientServices;
using FairPlayTube.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomProviders
{
    /// <summary>
    /// Provides the Access Token required to edit videos
    /// </summary>
    public class VideoEditAccessTokenProvider : IVideoEditAccessTokenProvider
    {
        private VideoClientService VideoClientService { get; }
        /// <summary>
        /// Initializes <see cref="VideoEditAccessTokenProvider"/>
        /// </summary>
        /// <param name="videoClientService"></param>
        public VideoEditAccessTokenProvider(VideoClientService videoClientService)
        {
            this.VideoClientService = videoClientService;
        }

        /// <summary>
        /// Returns the access token required to edit the given video
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public async Task<string> GetVideoEditAccessToken(string videoId)
        {
            return await this.VideoClientService.GetVideoEditAccessToken(videoId);
        }
    }
}
