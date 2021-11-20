using FairPlayTube.Common.Global.Enums.Localizers;
using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Common.Global.Enums
{
    public enum UserRequestType
    {
        [Display(
            Name = nameof(UserRequestTypeLocalizer.FeatureRequestDisplayName),
            ResourceType = typeof(UserRequestTypeLocalizer))]
        FeatureRequest=1,
        [Display(
            Name = nameof(UserRequestTypeLocalizer.ContentRequestDisplayName),
            ResourceType = typeof(UserRequestTypeLocalizer))]
        ContentRequest =2
    }
}