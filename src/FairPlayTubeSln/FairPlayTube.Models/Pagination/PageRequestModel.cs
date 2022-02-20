using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Pagination
{
    /// <summary>
    /// Handles the paging page request
    /// </summary>
    public class PageRequestModel
    {
        /// <summary>
        /// Page number beinb requested
        /// </summary>
        public int PageNumber { get; set; }
    }
}
