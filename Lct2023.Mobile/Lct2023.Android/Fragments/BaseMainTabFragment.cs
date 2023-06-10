using Android.OS;
using Android.Views;
using Google.Android.Material.Card;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding;

namespace Lct2023.Android.Fragments
{
    public abstract class BaseMainTabFragment<TViewModel> : BaseFragment<TViewModel>
        where TViewModel : BaseMainTabViewModel
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var set = CreateBindingSet();

            set.Bind(view.FindViewById<MaterialCardView>(Resource.Id.toolbar_image_container))
                .For(v => v.BindClick())
                .To(vm => vm.GoToProfileCommand);

            set.Apply();

            return view;
        }
    }
}
