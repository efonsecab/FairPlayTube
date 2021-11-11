using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.Invites;
using FairPlayTube.Models.UserAudience;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Notifications.Hubs;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data regarding a User
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private EmailService EmailService { get; }
        private MessageService MessageService { get; }
        private UserService UserService { get; }

        /// <summary>
        /// Initializes <see cref="UserController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="emailService"></param>
        /// <param name="messageService"></param>
        /// <param name="userService"></param>
        public UserController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider, EmailService emailService,
            MessageService messageService, UserService userService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
            this.EmailService = emailService;
            this.MessageService = messageService;
            this.UserService = userService;
        }

        /// <summary>
        /// Gets the name of the role assigned to the Logged In User
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize]
        public async Task<string> GetMyRole(CancellationToken cancellationToken)
        {
            var userAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var role = await this.FairplaytubeDatabaseContext.ApplicationUserRole
                .Include(p => p.ApplicationUser)
                .Include(p => p.ApplicationRole)
                .Where(p => p.ApplicationUser.AzureAdB2cobjectId.ToString() == userAdB2CObjectId)
                .Select(p => p.ApplicationRole.Name).SingleOrDefaultAsync(cancellationToken: cancellationToken);
            return role;
        }

        /// <summary>
        /// List the users in the system
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<UserModel[]> ListUsers(CancellationToken cancellationToken)
        {
            var result = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Include(p => p.VideoInfo)
                .Include(p => p.Brand)
                .Include(p => p.UserYouTubeChannel)
                .Select(p => new UserModel
                {
                    ApplicationUserId = p.ApplicationUserId,
                    Name = p.FullName,
                    BrandsCount = p.Brand.Count,
                    VideosCount = p.VideoInfo.Count,
                    YouTubeChannels = p.UserYouTubeChannel == null ? null : p.UserYouTubeChannel.Select(p => p.YouTubeChannelId).ToArray()
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        /// <summary>
        /// Invites a user to use the system
        /// </summary>
        /// <param name="inviteUserModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task InviteUser(InviteUserModel inviteUserModel, CancellationToken cancellationToken)
        {
            var userInvitation = await this.UserService.InviteUserAsync(inviteUserModel, cancellationToken: cancellationToken);
            var userName = this.CurrentUserProvider.GetUsername();
            StringBuilder completeBody = new(inviteUserModel.CustomMessage);
            completeBody.AppendLine();
            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}";
            var userHomePagePath = Common.Global.Constants.UserPagesRoutes.UserHomePage
                .Replace("{UserId:long}", userInvitation.InvitingApplicationUserId.ToString());
            string invitingUserHomeUrl = $"{baseUrl}{userHomePagePath}";
            string authPath = $"authentication/login?returnUrl={Uri.EscapeDataString(invitingUserHomeUrl)}";
            string fullLink = $"{baseUrl}/{authPath}";
            string link = $"<a href='{fullLink}'>{fullLink}</a>";
            completeBody.AppendLine($"Once you are on the website you can create your account using the Sign up link." +
                $"Your invite code is: {userInvitation.InviteCode}");
            completeBody.AppendLine(link);
            await this.EmailService.SendEmail(inviteUserModel.ToEmailAddress, $"{userName} is inviting you to " +
                $"FairPlayTube: The Next Generation Of Video Sharing Portals.",
                completeBody.ToString(), true);
        }

        /// <summary>
        /// Sends a message to the specified user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [FeatureGate(FeatureType.PaidFeature)]
        public async Task SendMessage(UserMessageModel model, CancellationToken cancellationToken)
        {
            var senderObjectId = this.CurrentUserProvider.GetObjectId();
            await this.MessageService.SendMessageAsync(model, senderObjectId, cancellationToken);
        }


        /// <summary>
        /// Adds a new user followed by the logged in user
        /// </summary>
        /// <param name="followedApplicationUserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Constants.Roles.User)]
        public async Task<IActionResult> AddUserFollower(long followedApplicationUserId, CancellationToken cancellationToken)
        {
            var userAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var followedUser = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.ApplicationUserId == followedApplicationUserId, cancellationToken: cancellationToken);
            if (followedUser == null)
                throw new CustomValidationException($"Invalid {nameof(followedApplicationUserId)}");
            await this.UserService.AddUserFollowerAsync(followerUserObjectId: userAdB2CObjectId,
                followedUserObjectId: followedUser.AzureAdB2cobjectId.ToString(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Gets a user status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Constants.Roles.User)]
        public async Task<string> GetMyUserStatus(CancellationToken cancellationToken)
        {
            var userAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var userStatus = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Include(p => p.ApplicationUserStatus)
                .Where(p => p.AzureAdB2cobjectId.ToString() == userAdB2CObjectId)
                .Select(p => p.ApplicationUserStatus.Name).SingleAsync(cancellationToken: cancellationToken);
            return userStatus;
        }

        /// <summary>
        /// Validates a user invite code
        /// </summary>
        /// <param name="userInviteCode"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Constants.Roles.User)]
        public async Task<IActionResult> ValidateUserInviteCode([FromQuery] string userInviteCode)
        {
            var userAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var userInvitation = await this.FairplaytubeDatabaseContext.UserInvitation
                .Where(p => p.InviteCode.ToString() == userInviteCode).SingleOrDefaultAsync();
            if (userInvitation != null)
            {
                var userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                    .Include(p => p.ApplicationUserStatus)
                    .Where(p => p.AzureAdB2cobjectId.ToString() == userAdB2CObjectId)
                    .SingleAsync();
                if (userEntity.EmailAddress.ToLower() != userInvitation.InvitedUserEmail.ToLower())
                {
                    throw new CustomValidationException("Account email does not match invite code email");
                }
                userEntity.ApplicationUserStatusId = 2;
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                return Ok();
            }
            throw new CustomValidationException("Invalid Invite Code");
        }
    }
}
