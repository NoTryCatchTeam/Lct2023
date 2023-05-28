using System;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.CheckBox;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.ProfTest;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.ProfTest;

[MvxDialogFragmentPresentation(Cancelable = true)]
public class ProfTestStep1Fragment : MvxDialogFragment<ProfTestStep1ViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.ProfTestStep1Fragment, container, false);

        var options = view.FindViewById<MvxRecyclerView>(Resource.Id.prof_test_step1_list);
        var start = view.FindViewById<MaterialButton>(Resource.Id.prof_test_step1_next);
        var close = view.FindViewById<MaterialButton>(Resource.Id.prof_test_step1_back);

        var optionsAdapter = new ProftestStep1OptionsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.checkbox_list_item),
        };

        options.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        options.SetAdapter(optionsAdapter);
        options.AddItemDecoration(new ColoredDividerItemDecoration(Activity, LinearLayoutManager.Vertical)
        {
            Drawable = Activity.GetDrawable(Resource.Drawable.simple_list_item_decorator),
        });

        var set = CreateBindingSet();

        set.Bind(optionsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.Options);

        set.Bind(start)
            .For(x => x.BindClick())
            .To(vm => vm.NextCommand);

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

public class ProftestStep1OptionsAdapter : BaseRecyclerViewAdapter<string, CheckboxViewHolder>
{
    public ProftestStep1OptionsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CheckboxViewHolder> BindableViewHolderCreator =>
        (v, c) => new CheckboxViewHolder(v, c);
}

public class CheckboxViewHolder : BaseRecyclerViewAdapter<string, CheckboxViewHolder>.BaseViewHolder
{
    public CheckboxViewHolder(View itemView, IMvxAndroidBindingContext context)
        : base(itemView, context)
    {
        var checkbox = itemView.FindViewById<MaterialCheckBox>(Resource.Id.checkbox_list_item_checkbox);

        this.DelayBind(() =>
        {
            var set = CreateBindingSet();

            set.Bind(checkbox)
                .For(x => x.Text)
                .To(vm => vm);

            set.Apply();
        });
    }
}
