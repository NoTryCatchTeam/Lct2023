using Android.OS;
using Android.Views;
using Google.Android.Material.Button;
using Lct2023.ViewModels.Anonymous;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Anonymous;

[MvxDialogFragmentPresentation(Cancelable = true)]
public class AnonymousWarningFragment : MvxDialogFragment<AnonymousWarningViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.AnonymousWarningFragment, container, false);

        var back = view.FindViewById<MaterialButton>(Resource.Id.anonymous_warning_close);
        var forward = view.FindViewById<MaterialButton>(Resource.Id.anonymous_warning_forward);

        var set = CreateBindingSet();

        set.Bind(back)
            .For(x => x.BindClick())
            .To(vm => vm.NavigateBackCommand);
        
        set.Bind(forward)
            .For(x => x.BindClick())
            .To(vm => vm.ContinueCommand);

        set.Apply();

        return view;
    }

    public override void OnStart()
    {
        base.OnStart();

        Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
    }
}
