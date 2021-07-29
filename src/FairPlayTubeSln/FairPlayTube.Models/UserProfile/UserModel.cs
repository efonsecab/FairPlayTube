namespace FairPlayTube.Models.UserProfile
{
    /// <summary>
    /// Represents the User entry
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// ApplicationUserId
        /// </summary>
        public long ApplicationUserId { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// # of Brands the user owns
        /// </summary>
        public long BrandsCount { get; set; }
        /// <summary>
        /// # of Videos the user owns
        /// </summary>
        public long VideosCount { get; set; }
    }
}
