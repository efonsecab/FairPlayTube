using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Invites
{
    public class InviteUserModel
    {
        [Required]
        [EmailAddress]
        public string ToEmailAddress { get; set; }
        [Required]
        public string CustomMessage { get; set; }
    }
}
