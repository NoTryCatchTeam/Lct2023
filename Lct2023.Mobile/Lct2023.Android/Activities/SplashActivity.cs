using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(
    Label = "@string/app_name",
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

    protected override async Task RunAppStartAsync(Bundle bundle)
    {
        await Task.Delay(300);

        await base.RunAppStartAsync(bundle);
    }
}
