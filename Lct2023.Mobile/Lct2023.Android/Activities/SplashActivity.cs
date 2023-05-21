using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(
    Label = "Lct2023",
    MainLauncher = true,
    NoHistory = true,
    ScreenOrientation = ScreenOrientation.Portrait)]
public class SplashActivity : MvxSplashScreenActivity<Setup, App>
{
    public SplashActivity() :
        base(Resource.Layout.SplashActivity)
    {
    }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        Xamarin.Essentials.Platform.Init(this, bundle);
    }
}
