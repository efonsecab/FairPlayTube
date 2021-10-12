using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Holds the information required to create a custom rendering project
    /// </summary>
    public class ProjectModel
    {
        /// <summary>
        /// The project's id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// Name of the Project
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Videos and ranges to be included for the custom project
        /// </summary>
        public ProjectVideoModel[] Videos { get; set; }
    }

    /// <summary>
    /// Halds the informtion for the time ranges for the specified VideoId
    /// </summary>
    public class ProjectVideoModel
    {
        /// <summary>
        /// Azure Video Indexer Video Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// Start Time
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// End Time
        /// </summary>
        public string End { get; set; }
    }
}
