using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Pagination
{
    /// <summary>
    /// Handles the returned paged items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedItems<T>
    {
        /// <summary>
        /// Page Number
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Total Items
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Items for page
        /// </summary>
        public T[] Items { get; set; }
        /// <summary>
        /// Total Pages
        /// </summary>
        public int TotalPages { get; set; }
    }
}
