﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(Type), nameof(Key), Name = "UI_Resource_Type_Key", IsUnique = true)]
    public partial class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        [Required]
        [StringLength(1500)]
        public string Type { get; set; }
        [Required]
        [StringLength(50)]
        public string Key { get; set; }
        [Required]
        [Column(TypeName = "text")]
        public string Value { get; set; }
        public int CultureId { get; set; }

        [ForeignKey(nameof(CultureId))]
        [InverseProperty("Resource")]
        public virtual Culture Culture { get; set; }
    }
}