using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.ClientSideErrorLog
{
    /// <summary>
    /// Holds the data to create a client side error log item
    /// </summary>
    public class CreateClientSideErrorLogModel
    {
        /// <summary>
        /// Error Message
        /// </summary>
        [Required]
        public string Message { get; set; }
        /// <summary>
        /// Error Stack Trace
        /// </summary>
        [Required]
        public string StackTrace { get; set; }
        /// <summary>
        /// Full Exception
        /// </summary>
        [Required]
        public string FullException { get; set; }
    }
}
