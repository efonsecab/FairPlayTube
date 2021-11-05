using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Validations.Video
{
    /// <summary>
    /// Holds the logic required to retrieve the localized values for <see cref="VideoJobModelLocalizer"/>
    /// </summary>
    public class VideoJobModelLocalizer
    {
        /// <summary>
        /// Typed localizer to retrieve the localized messages
        /// </summary>
        public static IStringLocalizer<VideoJobModelLocalizer> Localizer { get; set; }
        /// <summary>
        /// Retrieves the Title Display Name localized message
        /// </summary>
        public static string TitleDisplayName => Localizer[TitleDisplayNameTextKey];
        /// <summary>
        /// Retrieves the Budget Display Name
        /// </summary>
        public static string BudgetDisplayName => Localizer[BudgetDisplayNameTextKey];
        /// <summary>
        /// Retrieves the Description Display Name
        /// </summary>
        /// <returns></returns>
        public static string DescriptionDisplayName => Localizer[DescriptionDisplayNameTextKey];

        #region Resource Keys
        /// <summary>
        /// Resource key for Title Display Name
        /// </summary>
        [ResourceKey(defaultValue:"Title")]
        public const string TitleDisplayNameTextKey = "TitleDisplayNameText";
        /// <summary>
        /// Resource key for Budget
        /// </summary>
        [ResourceKey(defaultValue: "Budget")]
        public const string BudgetDisplayNameTextKey = "BudgetDisplayNameText";
        /// <summary>
        /// Resource ket for Description
        /// </summary>
        [ResourceKey(defaultValue: "Description")]
        public const string DescriptionDisplayNameTextKey = "DescriptionDisplayNameText";
        #endregion Resource Keys
    }
}
