using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private VideoService VideoService { get; }
        private IMapper Mapper { get; }

        public VideoController(VideoService videoService, IMapper mapper)
        {
            this.VideoService = videoService;
            this.Mapper = mapper;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<VideoInfoModel[]> GetPublicProcessedVideos()
        {
            var result = await this.VideoService.GetPublicProcessedVideosAsync()
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync();
            return result;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string> UploadVideo(UploadVideoModel uploadVideoModel)
        {
            return await this.VideoService.UploadVideoAsync(uploadVideoModel);
        }
    }
}
