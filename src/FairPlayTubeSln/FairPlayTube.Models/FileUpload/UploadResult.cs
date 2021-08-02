using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.FileUpload
{
    /// <summary>
    /// File upload result
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Error code received when uploading file
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Stored File name
        /// </summary>
        public string StoredFileName { get; set; }
        /// <summary>
        /// Indicates if the file has ben uploaded
        /// </summary>
        public bool Uploaded { get; set; }
    }
}
