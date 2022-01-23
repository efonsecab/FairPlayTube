using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Users
{
    /// <summary>
    /// Holds the data regarding an ApplicationUser
    /// </summary>
    public class ConversationsUserModel
    {
        /// <summary>
        /// If of an application user
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// User's Full Name
        /// </summary>
        [Required]
        [StringLength(150)]
        public string FullName { get; set; }
    }
}
