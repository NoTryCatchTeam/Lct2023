using System.Reactive.Disposables;
using Android.OS;
using Android.Views;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments;

public abstract class BaseFragment<TViewModel> : MvxFragment<TViewModel>
    where TViewModel : BaseViewModel
{
    protected CompositeDisposable CompositeDisposable { get; } = new ();
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        return this.BindingInflate(GetLayoutId(), container, false);
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
