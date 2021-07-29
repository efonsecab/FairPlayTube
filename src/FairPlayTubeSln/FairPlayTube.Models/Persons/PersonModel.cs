namespace FairPlayTube.Models.Persons
{
    /// <summary>
    /// Represents the Person Model from Azure Video Indexer
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// Azure Video Indexer Person Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Azure Video Indexer Person Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Azure Video Indexer Person Sample Face Id
        /// </summary>
        public string SampleFaceId { get; set; }
        /// <summary>
        /// Azure Video Indexer Person Sample Face Type
        /// </summary>
        public string SampleFaceSourceType { get; set; }
        /// <summary>
        /// Azure Video Indexer Person Sample Face State
        /// </summary>
        public string SampleFaceState { get; set; }
        /// <summary>
        /// Azure Video Indexer Person Person Model Id
        /// </summary>
        public string PersonModelId { get; set; }
        /// <summary>
        /// System Storage Url for the Sample Face
        /// </summary>
        public string SampleFaceUrl { get; set; }
    }
}
