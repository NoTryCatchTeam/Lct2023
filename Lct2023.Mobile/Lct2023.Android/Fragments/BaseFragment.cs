using System.Reactive.Disposables;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using Google.Android.Material.AppBar;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using Square.Picasso;

namespace Lct2023.Android.Fragments;

public abstract class BaseFragment<TViewModel> : MvxFragment<TViewModel>
    where TViewModel : BaseViewModel
{
    public BaseAppToolbar Toolbar { get; private set; }

    protected CompositeDisposable CompositeDisposable { get; } = new ();

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(GetLayoutId(), container, false);

        if (view.FindViewById<MaterialToolbar>(Resource.Id.toolbar) is { } imagedToolbarView)
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
        else if (view.FindViewById<MaterialToolbar>(Resource.Id.toolbar_inner) is { } innerToolbarView)
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

        return view;
    }

    protected abstract int GetLayoutId();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CompositeDisposable.Clear();
        }

        base.Dispose(disposing);
    }
}

public abstract class BaseFragment : MvxFragment
{
    protected CompositeDisposable CompositeDisposable { get; } = new ();

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(GetLayoutId(), container, false);

        return view;
    }

    protected abstract int GetLayoutId();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CompositeDisposable.Clear();
        }

        base.Dispose(disposing);
    }
}
