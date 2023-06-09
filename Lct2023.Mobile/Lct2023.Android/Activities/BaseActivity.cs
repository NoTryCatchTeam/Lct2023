using System.Reactive.Disposables;
using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using Google.Android.Material.AppBar;
using Lct2023.Android.Fragments;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Android.Views;
using Lct2023.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Square.Picasso;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity]
public abstract class BaseActivity<TViewModel> : MvxActivity<TViewModel>
    where TViewModel : BaseViewModel
{
    private BaseAppToolbar _toolbar;

    protected CompositeDisposable CompositeDisposable { get; } = new ();

    protected BaseAppToolbar Toolbar
    {
        get
        {
            if (_toolbar == null)
            {
                InitToolbar();
            }

            return _toolbar;
        }
        private set => _toolbar = value;
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CompositeDisposable.Clear();
        }

        base.Dispose(disposing);
    }

    private void InitToolbar()
    {
        if (FindViewById<MaterialToolbar>(Resource.Id.toolbar) is { } imagedToolbarView)
        {
            var toolbar = new ImagedToolbar(imagedToolbarView);

            if (ViewModel.UserContext.User?.PhotoUrl is { } photoUrl)
            {
                Picasso.Get()
                    .Load(Uri.Parse(photoUrl))
                    .Placeholder(Resource.Drawable.ic_profile_circle)
                    .Error(Resource.Drawable.ic_profile_circle)
                    .Into(
                        toolbar.Avatar,
                        () => toolbar.Avatar.ImageTintList = null,
                        _ =>
                        {
                        });
            }
            else
            {
                toolbar.Avatar.SetImageResource(Resource.Drawable.ic_profile_circle);
            }

            toolbar.Title = ViewModel.UserContext.User?.FirstName;

            var ratingMenuItem = toolbar.Toolbar.Menu.FindItem(Resource.Id.rating);
            ratingMenuItem.SetVisible(ViewModel.UserContext.IsAuthenticated);

            var set = CreateBindingSet();

            set.Bind(ratingMenuItem.ActionView.FindViewById<TextView>(Resource.Id.toolbar_rating_number))
                .For(x => x.Text)
                .To(vm => vm.UserContext.User.Rating);

            set.Apply();

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
}
