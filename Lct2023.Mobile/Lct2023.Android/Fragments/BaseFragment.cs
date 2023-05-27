using System.Reactive.Disposables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using Square.Picasso;

namespace Lct2023.Android.Fragments;

public abstract class BaseFragment<TViewModel> : MvxFragment<TViewModel>
    where TViewModel : BaseViewModel
{
    public AppToolbar Toolbar { get; private set; }

    protected CompositeDisposable CompositeDisposable { get; } = new ();

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(GetLayoutId(), container, false);

        if (view.FindViewById<MaterialToolbar>(Resource.Id.toolbar) is not { } toolbar)
        {
            return view;
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

public class AppToolbar
{
    public AppToolbar(MaterialToolbar toolbar)
    {
        Toolbar = toolbar;

        Avatar = toolbar.FindViewById<ImageView>(Resource.Id.toolbar_image);
        Title = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
    }

    public MaterialToolbar Toolbar { get; }

    public ImageView Avatar { get; }

    public TextView Title { get; }
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
