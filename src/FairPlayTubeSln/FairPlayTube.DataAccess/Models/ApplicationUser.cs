﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(AzureAdB2cobjectId), Name = "UI_ApplicationUser_AzureAdB2CObjectId", IsUnique = true)]
    public partial class ApplicationUser
    {
        public ApplicationUser()
        {
            ApplicationUserFeature = new HashSet<ApplicationUserFeature>();
            ApplicationUserRole = new HashSet<ApplicationUserRole>();
            Brand = new HashSet<Brand>();
            PaypalTransaction = new HashSet<PaypalTransaction>();
            UserExternalMonetization = new HashSet<UserExternalMonetization>();
            UserFeedback = new HashSet<UserFeedback>();
            UserFollowerFollowedApplicationUser = new HashSet<UserFollower>();
            UserFollowerFollowerApplicationUser = new HashSet<UserFollower>();
            UserInvitation = new HashSet<UserInvitation>();
            UserMessageFromApplicationUser = new HashSet<UserMessage>();
            UserMessageToApplicationUser = new HashSet<UserMessage>();
            UserProfile = new HashSet<UserProfile>();
            UserRequest = new HashSet<UserRequest>();
            UserYouTubeChannel = new HashSet<UserYouTubeChannel>();
            VideoAccessTransaction = new HashSet<VideoAccessTransaction>();
            VideoComment = new HashSet<VideoComment>();
            VideoInfo = new HashSet<VideoInfo>();
            VideoJobApplication = new HashSet<VideoJobApplication>();
            VideoPlaylist = new HashSet<VideoPlaylist>();
            VisitorTracking = new HashSet<VisitorTracking>();
        }

        [Key]
        public long ApplicationUserId { get; set; }
        [Required]
        [StringLength(150)]
        public string FullName { get; set; }
        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; }
        public DateTimeOffset LastLogIn { get; set; }
        [Column("AzureAdB2CObjectId")]
        public Guid AzureAdB2cobjectId { get; set; }
        [Column(TypeName = "money")]
        public decimal AvailableFunds { get; set; }
        public short ApplicationUserStatusId { get; set; }

        [ForeignKey(nameof(ApplicationUserStatusId))]
        [InverseProperty("ApplicationUser")]
        public virtual ApplicationUserStatus ApplicationUserStatus { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual UserVideoRating UserVideoRating { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<ApplicationUserFeature> ApplicationUserFeature { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<ApplicationUserRole> ApplicationUserRole { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<Brand> Brand { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<PaypalTransaction> PaypalTransaction { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<UserExternalMonetization> UserExternalMonetization { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<UserFeedback> UserFeedback { get; set; }
        [InverseProperty(nameof(UserFollower.FollowedApplicationUser))]
        public virtual ICollection<UserFollower> UserFollowerFollowedApplicationUser { get; set; }
        [InverseProperty(nameof(UserFollower.FollowerApplicationUser))]
        public virtual ICollection<UserFollower> UserFollowerFollowerApplicationUser { get; set; }
        [InverseProperty("InvitingApplicationUser")]
        public virtual ICollection<UserInvitation> UserInvitation { get; set; }
        [InverseProperty(nameof(UserMessage.FromApplicationUser))]
        public virtual ICollection<UserMessage> UserMessageFromApplicationUser { get; set; }
        [InverseProperty(nameof(UserMessage.ToApplicationUser))]
        public virtual ICollection<UserMessage> UserMessageToApplicationUser { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<UserProfile> UserProfile { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<UserRequest> UserRequest { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<UserYouTubeChannel> UserYouTubeChannel { get; set; }
        [InverseProperty("BuyerApplicationUser")]
        public virtual ICollection<VideoAccessTransaction> VideoAccessTransaction { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<VideoComment> VideoComment { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<VideoInfo> VideoInfo { get; set; }
        [InverseProperty("ApplicantApplicationUser")]
        public virtual ICollection<VideoJobApplication> VideoJobApplication { get; set; }
        [InverseProperty("OwnerApplicationUser")]
        public virtual ICollection<VideoPlaylist> VideoPlaylist { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<VisitorTracking> VisitorTracking { get; set; }
    }
}