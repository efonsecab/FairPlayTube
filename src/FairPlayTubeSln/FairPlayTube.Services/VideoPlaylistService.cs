using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Common.CustomExceptions;

namespace FairPlayTube.Services
{
    public class VideoPlaylistService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        public VideoPlaylistService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext, 
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task CreateVideoPlaylisyAsync(VideoPlaylistModel videoPlaylistModel, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var isDuplicated = await this.FairplaytubeDatabaseContext.VideoPlaylist.Include(p => p.OwnerApplicationUser)
                .AnyAsync(p => p.OwnerApplicationUser.AzureAdB2cobjectId.ToString() == userObjectId &&
                p.PlaylistName == videoPlaylistModel.PlaylistName, cancellationToken:cancellationToken);
            if (isDuplicated)
                throw new CustomValidationException($"User already has a playlist named '{videoPlaylistModel.PlaylistName}'");
            var userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            userObjectId).SingleAsync(cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.VideoPlaylist.AddAsync(new DataAccess.Models.VideoPlaylist() 
            {
                PlaylistName = videoPlaylistModel.PlaylistName,
                PlaylistDescription = videoPlaylistModel.PlaylistDescription,
                OwnerApplicationUserId = userEntity.ApplicationUserId
            }, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteVideoPlaylistAsync(long videoPlaylistId, CancellationToken cancellationToken)
        {
            var videoplaylistEntity = await this.FairplaytubeDatabaseContext.VideoPlaylist
                .Include(p=>p.OwnerApplicationUser)
                .SingleOrDefaultAsync(p => p.VideoPlaylistId == videoPlaylistId, cancellationToken: cancellationToken);
            if (videoplaylistEntity == null)
                throw new CustomValidationException($"Unable to find videoplaylist with id: {videoPlaylistId}");
            var userobjectId = this.CurrentUserProvider.GetObjectId();
            if (userobjectId != videoplaylistEntity.OwnerApplicationUser.AzureAdB2cobjectId.ToString())
                throw new CustomValidationException("Access denied. User does not own the specified videoplaylist");
            this.FairplaytubeDatabaseContext.VideoPlaylist.Remove(videoplaylistEntity);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<VideoPlaylistItem> AddVideoToPlaylist(VideoPlaylistItemModel videoPlaylistItemModel, 
            CancellationToken cancellationToken)
        {
            var videoEntity = await FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p => p.VideoId == videoPlaylistItemModel.VideoId, cancellationToken: cancellationToken);
            var videoplaylistEntity = await this.FairplaytubeDatabaseContext.VideoPlaylist
                .Include(p=>p.OwnerApplicationUser)
                .SingleOrDefaultAsync(p => p.VideoPlaylistId == 
                videoPlaylistItemModel.VideoPlaylistId, cancellationToken: cancellationToken);
            var userobjectId = this.CurrentUserProvider.GetObjectId();
            if (userobjectId != videoplaylistEntity.OwnerApplicationUser.AzureAdB2cobjectId.ToString())
                throw new CustomValidationException("Access denied. User does not own the specified videoplaylist");
            var videoPlayListItemEntity = new VideoPlaylistItem()
            {
                VideoPlaylistId = videoPlaylistItemModel.VideoPlaylistId,
                VideoInfoId = videoEntity.VideoInfoId,
                Order = videoPlaylistItemModel.Order
            };
            await this.FairplaytubeDatabaseContext.VideoPlaylistItem.AddAsync(videoPlayListItemEntity, cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            return videoPlayListItemEntity;
        }
    }
}
