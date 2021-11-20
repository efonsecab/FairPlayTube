using FairPlayTube.Common.Global.Enums;
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
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
