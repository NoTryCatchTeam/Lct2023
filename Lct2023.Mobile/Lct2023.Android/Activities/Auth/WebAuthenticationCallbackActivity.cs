using Android.App;
using Android.Content.PM;

namespace Lct2023.Android.Activities.Auth;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    new[]
    {
        global::Android.Content.Intent.ActionView,
    },
    DataSchemes = new[] { "lct2023" },
    Categories = new[]
    {
        global::Android.Content.Intent.CategoryDefault,
        global::Android.Content.Intent.CategoryBrowsable,
    })]
public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
{
}
