using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Models.UsersRequests.Localizers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UsersRequests
{
    /// <summary>
    /// Holds the data to create a user request
    /// </summary>
    public class CreateUserRequestModel
    {
        /// <summary>
        /// Indicates the type of request
        /// </summary>
        public UserRequestType UserRequestType { get; set; }
        /// <summary>
        /// The details for the request
        /// </summary>
        [Required(
            ErrorMessageResourceName =nameof(CreateUserRequestModelLocalizer.DescriptionRequired),
            ErrorMessageResourceType =typeof(CreateUserRequestModelLocalizer))]
        [StringLength(1000,
            ErrorMessageResourceName = nameof(CreateUserRequestModelLocalizer.DescriptionTooLong),
            ErrorMessageResourceType = typeof(CreateUserRequestModelLocalizer))]
        public string Description { get; set; }
        /// <summary>
        /// User Email Address
        /// </summary>
        [Required(
            ErrorMessageResourceName = nameof(CreateUserRequestModelLocalizer.EmailAddressRequired),
            ErrorMessageResourceType = typeof(CreateUserRequestModelLocalizer))]
        [StringLength(150,
            ErrorMessageResourceName = nameof(CreateUserRequestModelLocalizer.EmailAddressTooLong),
            ErrorMessageResourceType = typeof(CreateUserRequestModelLocalizer))]
        [EmailAddress(
            ErrorMessageResourceName =nameof(CreateUserRequestModelLocalizer.EmailAddressFormat),
            ErrorMessageResourceType =typeof(CreateUserRequestModelLocalizer))]
        public string EmailAddress { get; set; }
    }
}
