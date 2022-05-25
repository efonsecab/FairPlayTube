namespace FairPlayTube.Models.RoleRequest
{
    /// <summary>
    /// Holds the information for a Role Request
    /// </summary>
    public class AddRoleRequestModel
    {
        /// <summary>Role Request Name</summary>
        public string Name { get; set; }
        /// <summary>Role Request Last Name</summary>
        public string Lastname { get; set; }
        /// <summary>Role Request Last Surname</summary>
        public string Surname { get; set; }
        /// <summary>Role Request Nationality</summary>
        public string Nationality { get; set; }
        /// <summary>Role Request NationalId</summary>
        public int NationalId { get; set; }
        /// <summary>Role Request National Id Photo Stored File Name</summary>
        public string NationalIdPhotoStoredFileName { get; set; }
        /// <summary>Role Request Request Application Role</summary>
        public int RequestedApplicationRole { get; set; }
    }
}
