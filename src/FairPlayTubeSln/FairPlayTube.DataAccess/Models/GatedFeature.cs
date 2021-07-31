﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FairPlayTube.DataAccess.Models
{
    [Index(nameof(FeatureName), Name = "UI_GatedFeature_FeatureName", IsUnique = true)]
    public partial class GatedFeature
    {
        [Key]
        public int GatedFeatureId { get; set; }
        [Required]
        [StringLength(250)]
        public string FeatureName { get; set; }
        [Required]
        public bool? DefaultValue { get; set; }
    }
}