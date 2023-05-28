using System.Reactive.Disposables;
using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Google.Android.Material.AppBar;
using Lct2023.Android.Fragments;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
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

    protected BaseAppToolbar Toolbar { get; private set; }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (FindViewById<MaterialToolbar>(Resource.Id.toolbar) is { } imagedToolbarView)
        {
            var toolbar = new ImagedToolbar(imagedToolbarView);

            if (ViewModel.UserContext.User?.PhotoUrl is { } photoUrl)
            {
                Picasso.Get()
                    .Load(Uri.Parse(photoUrl))
                    .Placeholder(Resource.Drawable.ic_profile_circle)
                    .Error(Resource.Drawable.ic_profile_circle)
                    .Into(toolbar.Avatar);
            }
            else
            {
                toolbar.Avatar.SetImageResource(Resource.Drawable.ic_profile_circle);
            }

            toolbar.Title = ViewModel.UserContext.User?.FirstName;

            Toolbar = toolbar;
        }
        else if (FindViewById<MaterialToolbar>(Resource.Id.toolbar_inner) is { } innerToolbarView)
        {
            var toolbar = new InnerToolbar(innerToolbarView)
            {
                Title = ViewModel.UserContext.User?.FirstName,
            };

            toolbar.Toolbar.SetOnMenuItemClickListener(new DefaultMenuItemClickListener(_ =>
            {
                ViewModel.NavigateBackCommand.ExecuteAsync();

                return true;
            }));

            Toolbar = toolbar;
        }
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
