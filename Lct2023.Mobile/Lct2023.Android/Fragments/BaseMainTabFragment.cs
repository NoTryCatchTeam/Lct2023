using System;
using Android.OS;
using Android.Views;
using Google.Android.Material.AppBar;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using System.Reactive.Disposables;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Square.Picasso;
using MvvmCross.Platforms.Android.Binding;

namespace Lct2023.Android.Fragments
{
	public abstract class BaseMainTabFragment<TViewModel> : BaseFragment<TViewModel>
		where TViewModel: BaseMainTabViewModel
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var toolbar = (ImagedToolbar)Toolbar;

            var set = CreateBindingSet();

            set
                .Bind(toolbar.Avatar)
                .For(v => v.BindClick())
                .To(vm => vm.GoToProfileCommand);

            set.Apply();

            return view;
        }
    }
}

