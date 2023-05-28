using Android.OS;
using Android.Views;
using Google.Android.Material.Button;
using Lct2023.ViewModels.ProfTest;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.ProfTest;

[MvxDialogFragmentPresentation(Cancelable = true)]
public class ProfTestFinishFragment : MvxDialogFragment<ProfTestFinishViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.ProfTestFinishFragment, container, false);

        var close = view.FindViewById<MaterialButton>(Resource.Id.prof_test_finish_next);

        var set = CreateBindingSet();

        set.Bind(close)
            .For(x => x.BindClick())
            .To(vm => vm.NavigateBackCommand);

        set.Apply();

        return view;
    }

    public override void OnStart()
    {
        base.OnStart();

        Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
    }
}
