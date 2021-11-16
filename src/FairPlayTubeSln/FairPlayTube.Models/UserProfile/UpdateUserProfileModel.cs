using FairPlayTube.Models.UserProfile.Localizers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserProfile
{
    /// <summary>
    /// Holds the data related to a user's profile
    /// </summary>
    public class UpdateUserProfileModel
    {
        /// <summary>
        /// Information about the user
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.AboutRequired),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [StringLength(1000, 
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.AboutTooLong),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [Display(Name = nameof(UpdateUserProfileModelLocalizer.AboutDisplayName),
            ResourceType = typeof(UpdateUserProfileModelLocalizer))]
        public string About { get; set; }
        /// <summary>
        /// Topics the user likes
        /// </summary>
        [Required(ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.TopicsRequired),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [StringLength(1000, 
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.TopicsTooLong),
            ErrorMessageResourceType =typeof(UpdateUserProfileModelLocalizer))]
        [Display(Name = nameof(UpdateUserProfileModelLocalizer.TopicsDisplayName),
            ResourceType =typeof(UpdateUserProfileModelLocalizer))]
        public string Topics { get; set; }
        /// <summary>
        /// User's nationality
        /// </summary>
        [Required(
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.NationalityRequired),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [StringLength(20, 
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.NationalityTooLong),
            ErrorMessageResourceType =typeof(UpdateUserProfileModelLocalizer))]
        [Display(
            Name = nameof(UpdateUserProfileModelLocalizer.NationalityDisplayName),
            ResourceType = typeof(UpdateUserProfileModelLocalizer))]
        public string Nationality { get; set; }
        /// <summary>
        /// User's National Id
        /// </summary>
        [StringLength(50, 
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.NationalIdTooLong),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [Display(
            Name = nameof(UpdateUserProfileModelLocalizer.NationalIdNumberDisplayName),
            ResourceType = typeof(UpdateUserProfileModelLocalizer))]
        public string NationalIdNumber { get; set; }
        /// <summary>
        /// User's National Id Photo
        /// </summary>
        [StringLength(200)]
        public string NationalIdPhotoUrl { get; set; }
        /// <summary>
        /// Alias the user want to be displayed as within the system
        /// </summary>
        [StringLength(100, 
            ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.AliasTooLong),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [Display(
            Name = nameof(UpdateUserProfileModelLocalizer.AliasDisplayName),
            ResourceType = typeof(UpdateUserProfileModelLocalizer))]
        public string DisplayAlias { get; set; }
        /// <summary>
        /// User paypal email address, used for payouts
        /// </summary>
        [StringLength(500)]
        [EmailAddress(ErrorMessageResourceName = nameof(UpdateUserProfileModelLocalizer.PaypalEmailAddressFormatError),
            ErrorMessageResourceType = typeof(UpdateUserProfileModelLocalizer))]
        [Display(Name = nameof(UpdateUserProfileModelLocalizer.PaypalEmailAddressDisplayName),
            ResourceType = typeof(UpdateUserProfileModelLocalizer))]
        public string PaypalEmailAddress { get; set; }
    }
}
