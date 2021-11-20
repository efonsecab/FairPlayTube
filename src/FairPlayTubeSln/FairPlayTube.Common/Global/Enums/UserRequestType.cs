using System.ComponentModel.DataAnnotations;

namespace FairPlayTube.Common.Global.Enums
{
    public enum UserRequestType
    {
        [Display(Name = "Feature Request")]
        FeatureRequest=1,
        [Display(Name = "Content Request")]
        ContentRequest=2
    }
}