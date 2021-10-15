﻿using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoJobService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        public VideoJobService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        public async Task AddVideoJobAsync(VideoJobModel videoJobModel, CancellationToken cancellationToken)
        {
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.VideoId == videoJobModel.VideoId, cancellationToken: cancellationToken);
            if (videoEntity == null)
                throw new Exception($"Video with id: {videoJobModel.VideoId} does not exist");
            await this.FairplaytubeDatabaseContext.VideoJob.AddAsync(new VideoJob()
            {
                Budget = videoJobModel.Budget,
                Title = videoJobModel.Title,
                Description = videoJobModel.Description,
                VideoInfoId = videoEntity.VideoInfoId
            }, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }


        public IQueryable<VideoJob> GetVideosJobs()
        {
            return this.FairplaytubeDatabaseContext.VideoJob.Include(p=>p.VideoInfo);
        }
    }
}