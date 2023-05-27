using System.Reactive.Disposables;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Google.Android.Material.AppBar;
using Lct2023.Android.Fragments;
using Lct2023.Android.Views;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Square.Picasso;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity]
public abstract class BaseActivity<TViewModel> : MvxActivity<TViewModel>
    where TViewModel : BaseViewModel
{
    protected CompositeDisposable CompositeDisposable { get; } = new ();

    protected AppToolbar Toolbar { get; private set; }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (FindViewById<MaterialToolbar>(Resource.Id.toolbar) is not { } toolbar)
        {
            return;
        }

        Toolbar = new AppToolbar(toolbar);

        var set = CreateBindingSet();

        if (ViewModel.UserContext.User?.PhotoUrl is { } photoUrl)
        {
            Picasso.Get()
                .Load(photoUrl)
                .Placeholder(Resource.Drawable.ic_profile_circle)
                .Error(Resource.Drawable.ic_profile_circle)
                .Into(Toolbar.Avatar);
        }
        else
        {
            Toolbar.Avatar.SetImageResource(Resource.Drawable.ic_profile_circle);
        }

        set.Bind(Toolbar.Title)
            .For(x => x.Text)
            .To(x => x.UserContext.User.FirstName);

        set.Apply();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CompositeDisposable.Clear();
        }

        base.Dispose(disposing);
    }
}
