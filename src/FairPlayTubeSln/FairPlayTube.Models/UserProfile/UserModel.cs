using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.UserProfile
{
    public class UserModel
    {
        public long ApplicationUserId { get; set; }
        public string Name { get; set; }
        public long BrandsCount { get; set; }
        public long VideosCount { get; set; }
    }
}
