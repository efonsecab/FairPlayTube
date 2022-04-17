﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FairPlayTube.DataAccess.Models;

namespace FairPlayTube.DataAccess.Data
{
    public partial class FairplaytubeDatabaseContext : DbContext
    {
        public FairplaytubeDatabaseContext()
        {
        }

        public FairplaytubeDatabaseContext(DbContextOptions<FairplaytubeDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationRole> ApplicationRole { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }
        public virtual DbSet<ApplicationUserApiRequest> ApplicationUserApiRequest { get; set; }
        public virtual DbSet<ApplicationUserFeature> ApplicationUserFeature { get; set; }
        public virtual DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
        public virtual DbSet<ApplicationUserStatus> ApplicationUserStatus { get; set; }
        public virtual DbSet<ApplicationUserSubscriptionPlan> ApplicationUserSubscriptionPlan { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<BrandVideo> BrandVideo { get; set; }
        public virtual DbSet<ClientSideErrorLog> ClientSideErrorLog { get; set; }
        public virtual DbSet<Culture> Culture { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<GatedFeature> GatedFeature { get; set; }
        public virtual DbSet<PaypalPayoutBatch> PaypalPayoutBatch { get; set; }
        public virtual DbSet<PaypalPayoutBatchItem> PaypalPayoutBatchItem { get; set; }
        public virtual DbSet<PaypalTransaction> PaypalTransaction { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
        public virtual DbSet<UserExternalMonetization> UserExternalMonetization { get; set; }
        public virtual DbSet<UserFeedback> UserFeedback { get; set; }
        public virtual DbSet<UserFollower> UserFollower { get; set; }
        public virtual DbSet<UserInvitation> UserInvitation { get; set; }
        public virtual DbSet<UserMessage> UserMessage { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserRequest> UserRequest { get; set; }
        public virtual DbSet<UserRequestType> UserRequestType { get; set; }
        public virtual DbSet<UserVerificationStatus> UserVerificationStatus { get; set; }
        public virtual DbSet<UserVideoRating> UserVideoRating { get; set; }
        public virtual DbSet<UserYouTubeChannel> UserYouTubeChannel { get; set; }
        public virtual DbSet<VideoAccessTransaction> VideoAccessTransaction { get; set; }
        public virtual DbSet<VideoComment> VideoComment { get; set; }
        public virtual DbSet<VideoCommentAnalysis> VideoCommentAnalysis { get; set; }
        public virtual DbSet<VideoIndexKeyword> VideoIndexKeyword { get; set; }
        public virtual DbSet<VideoIndexStatus> VideoIndexStatus { get; set; }
        public virtual DbSet<VideoIndexingCost> VideoIndexingCost { get; set; }
        public virtual DbSet<VideoIndexingMargin> VideoIndexingMargin { get; set; }
        public virtual DbSet<VideoIndexingTransaction> VideoIndexingTransaction { get; set; }
        public virtual DbSet<VideoInfo> VideoInfo { get; set; }
        public virtual DbSet<VideoJob> VideoJob { get; set; }
        public virtual DbSet<VideoJobApplication> VideoJobApplication { get; set; }
        public virtual DbSet<VideoJobApplicationStatus> VideoJobApplicationStatus { get; set; }
        public virtual DbSet<VideoJobEscrow> VideoJobEscrow { get; set; }
        public virtual DbSet<VideoPlaylist> VideoPlaylist { get; set; }
        public virtual DbSet<VideoPlaylistItem> VideoPlaylistItem { get; set; }
        public virtual DbSet<VideoVisibility> VideoVisibility { get; set; }
        public virtual DbSet<VisitorTracking> VisitorTracking { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=FairPlayTube.Database;Integrated Security=true");

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.ApplicationUserStatusId).HasDefaultValueSql("1");

                entity.HasOne(d => d.ApplicationUserStatus)
                    .WithMany(p => p.ApplicationUser)
                    .HasForeignKey(d => d.ApplicationUserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUser_ApplicationUserStatus");
            });

            modelBuilder.Entity<ApplicationUserApiRequest>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.ApplicationUserApiRequest)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserApiRequest_ApplicationUser");
            });

            modelBuilder.Entity<ApplicationUserFeature>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.ApplicationUserFeature)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserFeature_ApplicationUser");

                entity.HasOne(d => d.GatedFeature)
                    .WithMany(p => p.ApplicationUserFeature)
                    .HasForeignKey(d => d.GatedFeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserFeature_GatedFeature");
            });

            modelBuilder.Entity<ApplicationUserRole>(entity =>
            {
                entity.HasOne(d => d.ApplicationRole)
                    .WithMany(p => p.ApplicationUserRole)
                    .HasForeignKey(d => d.ApplicationRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserRole_ApplicationRole");

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.ApplicationUserRole)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserRole_ApplicationUser");
            });

            modelBuilder.Entity<ApplicationUserStatus>(entity =>
            {
                entity.Property(e => e.ApplicationUserStatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<ApplicationUserSubscriptionPlan>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithOne(p => p.ApplicationUserSubscriptionPlan)
                    .HasForeignKey<ApplicationUserSubscriptionPlan>(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserSubscriptionPlan_ApplicationUser");

                entity.HasOne(d => d.SubscriptionPlan)
                    .WithMany(p => p.ApplicationUserSubscriptionPlan)
                    .HasForeignKey(d => d.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserSubscriptionPlan_SubscriptionPlan");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.Brand)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_ApplicationUserId");
            });

            modelBuilder.Entity<BrandVideo>(entity =>
            {
                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandVideo)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandVideo_Brand");

                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.BrandVideo)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandVideo_VideoInfo");
            });

            modelBuilder.Entity<Culture>(entity =>
            {
                entity.Property(e => e.CultureId).ValueGeneratedNever();
            });

            modelBuilder.Entity<GatedFeature>(entity =>
            {
                entity.Property(e => e.DefaultValue).HasDefaultValueSql("1");
            });

            modelBuilder.Entity<PaypalPayoutBatchItem>(entity =>
            {
                entity.HasOne(d => d.PaypalPayoutBatch)
                    .WithMany(p => p.PaypalPayoutBatchItem)
                    .HasForeignKey(d => d.PaypalPayoutBatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaypalPayoutBatchItem_PaypalPayoutBatch");
            });

            modelBuilder.Entity<PaypalTransaction>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.PaypalTransaction)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaypalTransaction_ApplicationUser");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasOne(d => d.Culture)
                    .WithMany(p => p.Resource)
                    .HasForeignKey(d => d.CultureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resource_Culture");
            });

            modelBuilder.Entity<UserExternalMonetization>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserExternalMonetization)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserExternalMonetization_ApplicationUser");
            });

            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserFeedback)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedback_ApplicationUserId");
            });

            modelBuilder.Entity<UserFollower>(entity =>
            {
                entity.HasOne(d => d.FollowedApplicationUser)
                    .WithMany(p => p.UserFollowerFollowedApplicationUser)
                    .HasForeignKey(d => d.FollowedApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFollower_FollowedApplicationUserId");

                entity.HasOne(d => d.FollowerApplicationUser)
                    .WithMany(p => p.UserFollowerFollowerApplicationUser)
                    .HasForeignKey(d => d.FollowerApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFollower_FollowerApplicationUserId");
            });

            modelBuilder.Entity<UserInvitation>(entity =>
            {
                entity.HasOne(d => d.InvitingApplicationUser)
                    .WithMany(p => p.UserInvitation)
                    .HasForeignKey(d => d.InvitingApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserInvitation_InvitingApplicationUserId");
            });

            modelBuilder.Entity<UserMessage>(entity =>
            {
                entity.HasOne(d => d.FromApplicationUser)
                    .WithMany(p => p.UserMessageFromApplicationUser)
                    .HasForeignKey(d => d.FromApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FromApplicationUserId_ApplicationUser");

                entity.HasOne(d => d.ToApplicationUser)
                    .WithMany(p => p.UserMessageToApplicationUser)
                    .HasForeignKey(d => d.ToApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ToApplicationUserId_ApplicationUser");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserProfile)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserId_UserProfile");

                entity.HasOne(d => d.UserVerificationStatus)
                    .WithMany(p => p.UserProfile)
                    .HasForeignKey(d => d.UserVerificationStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProfile_VerificationStatus");
            });

            modelBuilder.Entity<UserRequest>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserRequest)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .HasConstraintName("FK_UserRequest_ApplicationUser");

                entity.HasOne(d => d.UserRequestType)
                    .WithMany(p => p.UserRequest)
                    .HasForeignKey(d => d.UserRequestTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRequest_UserRequestType");
            });

            modelBuilder.Entity<UserRequestType>(entity =>
            {
                entity.Property(e => e.UserRequestTypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserVideoRating>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithOne(p => p.UserVideoRating)
                    .HasForeignKey<UserVideoRating>(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserVideoRating_ApplicationUserId");

                entity.HasOne(d => d.VideoInfo)
                    .WithOne(p => p.UserVideoRating)
                    .HasForeignKey<UserVideoRating>(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserVideoRating_VideoInfoId");
            });

            modelBuilder.Entity<UserYouTubeChannel>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserYouTubeChannel)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserYouTubeChannel_ApplicationUser");
            });

            modelBuilder.Entity<VideoAccessTransaction>(entity =>
            {
                entity.HasOne(d => d.BuyerApplicationUser)
                    .WithMany(p => p.VideoAccessTransaction)
                    .HasForeignKey(d => d.BuyerApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoAccessTransaction_ApplicationUser");

                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoAccessTransaction)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoAccessTransaction_VideoInfo");
            });

            modelBuilder.Entity<VideoComment>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.VideoComment)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUserId_VideoComment");

                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoComment)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoInfo_VideoComment");
            });

            modelBuilder.Entity<VideoCommentAnalysis>(entity =>
            {
                entity.HasOne(d => d.VideoComment)
                    .WithOne(p => p.VideoCommentAnalysis)
                    .HasForeignKey<VideoCommentAnalysis>(d => d.VideoCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoCommentAnalysis_VideoComment");
            });

            modelBuilder.Entity<VideoIndexKeyword>(entity =>
            {
                entity.HasKey(e => e.VideoIndexKwywordId)
                    .HasName("PK_VideoIndex");

                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoIndexKeyword)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoIndexKeyword_VideoInfo");
            });

            modelBuilder.Entity<VideoIndexStatus>(entity =>
            {
                entity.Property(e => e.VideoIndexStatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<VideoIndexingTransaction>(entity =>
            {
                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoIndexingTransaction)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoIndexingTransaction_VideoInfo");
            });

            modelBuilder.Entity<VideoInfo>(entity =>
            {
                entity.Property(e => e.ApplicationUserId).HasComment("Video Owner Id");

                entity.Property(e => e.OriginatorIpaddress).HasDefaultValueSql("'unknown'");

                entity.Property(e => e.RowCreationDateTime).HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.RowCreationUser).HasDefaultValueSql("'unknown'");

                entity.Property(e => e.SourceApplication).HasDefaultValueSql("'unknown'");

                entity.Property(e => e.VideoVisibilityId).HasDefaultValueSql("1");

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.VideoInfo)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoInfo_ApplicationUser");

                entity.HasOne(d => d.VideoIndexStatus)
                    .WithMany(p => p.VideoInfo)
                    .HasForeignKey(d => d.VideoIndexStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoInfo_VideoIndexStatus");

                entity.HasOne(d => d.VideoVisibility)
                    .WithMany(p => p.VideoInfo)
                    .HasForeignKey(d => d.VideoVisibilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoInfo_VideoVisibility");
            });

            modelBuilder.Entity<VideoJob>(entity =>
            {
                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoJob)
                    .HasForeignKey(d => d.VideoInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoJob_VideoInfo");
            });

            modelBuilder.Entity<VideoJobApplication>(entity =>
            {
                entity.HasOne(d => d.ApplicantApplicationUser)
                    .WithMany(p => p.VideoJobApplication)
                    .HasForeignKey(d => d.ApplicantApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoJobApplication_ApplicationUser");

                entity.HasOne(d => d.VideoJobApplicationStatus)
                    .WithMany(p => p.VideoJobApplication)
                    .HasForeignKey(d => d.VideoJobApplicationStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoJobApplication_VideoJobApplicationStatus");

                entity.HasOne(d => d.VideoJob)
                    .WithMany(p => p.VideoJobApplication)
                    .HasForeignKey(d => d.VideoJobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoJobApplication_VideoJobId");
            });

            modelBuilder.Entity<VideoJobApplicationStatus>(entity =>
            {
                entity.Property(e => e.VideoJobApplicationStatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<VideoJobEscrow>(entity =>
            {
                entity.HasOne(d => d.PaypalPayoutBatchItem)
                    .WithMany(p => p.VideoJobEscrow)
                    .HasForeignKey(d => d.PaypalPayoutBatchItemId)
                    .HasConstraintName("FK_VideoJobEscrow_PaypalPayoutBatchItem");

                entity.HasOne(d => d.VideoJob)
                    .WithOne(p => p.VideoJobEscrow)
                    .HasForeignKey<VideoJobEscrow>(d => d.VideoJobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoJobEscrow_VideoJob");
            });

            modelBuilder.Entity<VideoPlaylist>(entity =>
            {
                entity.HasOne(d => d.OwnerApplicationUser)
                    .WithMany(p => p.VideoPlaylist)
                    .HasForeignKey(d => d.OwnerApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoPlaylist_ApplicationUser");
            });

            modelBuilder.Entity<VideoPlaylistItem>(entity =>
            {
                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VideoPlaylistItem)
                    .HasForeignKey(d => d.VideoInfoId)
                    .HasConstraintName("FK_VideoPlaylistItem_VideoInfo");

                entity.HasOne(d => d.VideoPlaylist)
                    .WithMany(p => p.VideoPlaylistItem)
                    .HasForeignKey(d => d.VideoPlaylistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VideoPlaylistItem_VideoPlaylist");
            });

            modelBuilder.Entity<VideoVisibility>(entity =>
            {
                entity.Property(e => e.VideoVisibilityId).ValueGeneratedNever();
            });

            modelBuilder.Entity<VisitorTracking>(entity =>
            {
                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.VisitorTracking)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .HasConstraintName("FK_VisitorTracking_ApplicationUser");

                entity.HasOne(d => d.VideoInfo)
                    .WithMany(p => p.VisitorTracking)
                    .HasForeignKey(d => d.VideoInfoId)
                    .HasConstraintName("FK_VisitorTracking_VideoInfo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}