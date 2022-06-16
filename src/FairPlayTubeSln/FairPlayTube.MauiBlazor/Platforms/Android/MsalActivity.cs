using Android.App;
using Android.Content.PM;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
#if DEBUG
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView },
        Categories = new[] { global::Android.Content.Intent.CategoryBrowsable,
            global::Android.Content.Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal110facd2-07c2-4ecd-80e4-aeb3627100cf")]
#else
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView },
            Categories = new[] { global::Android.Content.Intent.CategoryBrowsable,
                global::Android.Content.Intent.CategoryDefault },
            DataHost = "auth",
            DataScheme = "msal3fe0e8b7-12c1-47e5-bd6a-63b47504c5ee")]
#endif
    public class MsalActivity : BrowserTabActivity
    {
    }
}
