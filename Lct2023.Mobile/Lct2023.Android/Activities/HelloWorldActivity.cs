using Android.App;
using Android.Content.PM;
using Android.OS;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class HelloWorldActivity : MvxActivity<HelloWorldViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.HelloWorldActivity);
    }
}
