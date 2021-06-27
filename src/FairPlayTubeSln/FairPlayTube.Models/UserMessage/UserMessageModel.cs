using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserMessage
{
    public class UserMessageModel
    {
        public long ToApplicationUserId { get; set; }
        public string Message { get; set; }
    }
}
