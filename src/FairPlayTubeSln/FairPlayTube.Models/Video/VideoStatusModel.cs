namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Represents a Video Status
    /// </summary>
    public class VideoStatusModel
    {
        /// <summary>
        /// Video Id
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Processing progress
        /// </summary>
        public string ProcessingProgress { get; set; }
    }
}
