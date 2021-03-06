// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.DataAccess.Models
{
    public partial class UserRequest
    {
        [Key]
        public long UserRequestId { get; set; }
        public short UserRequestTypeId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; }
        public long? ApplicationUserId { get; set; }
        public DateTimeOffset RowCreationDateTime { get; set; }
        [Required]
        [StringLength(256)]
        public string RowCreationUser { get; set; }
        [Required]
        [StringLength(250)]
        public string SourceApplication { get; set; }
        [Required]
        [Column("OriginatorIPAddress")]
        [StringLength(100)]
        public string OriginatorIpaddress { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        [InverseProperty("UserRequest")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(UserRequestTypeId))]
        [InverseProperty("UserRequest")]
        public virtual UserRequestType UserRequestType { get; set; }
    }
}