using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Lct2023.ViewModels.Common;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Activities.Common;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class WebViewActivity : BaseActivity<WebViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.WebViewActivity);

        var webView = FindViewById<WebView>(Resource.Id.web_view);
        webView.SetWebViewClient(new WebViewClient());
        webView.Settings.JavaScriptEnabled = true;
        webView.LoadUrl(ViewModel.NavigationParameter.Url);
    }

    protected override void OnResume()
    {
        base.OnResume();

        Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
    }

    protected override void OnPause()
    {
        base.OnPause();

        Window.ClearFlags(WindowManagerFlags.Secure);
    }
}
