using System;
using System.Windows.Input;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.CheckBox;
using Lct2023.Converters;
using Lct2023.ViewModels.Map.Filters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class MapFiltersSubGroupsListAdapter : BaseRecyclerViewAdapter<MapFilterSubGroupItemViewModel, MapFiltersSubGroupsListAdapter.MapFilterSubGroupViewHolder>
{
    public MapFiltersSubGroupsListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, MapFilterSubGroupViewHolder> BindableViewHolderCreator =>
        (v, c) => new MapFilterSubGroupViewHolder(v, c);

    public class MapFilterSubGroupViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _recyclerView;
        private MvxRecyclerAdapter _filtersItemAdapter;

        public MapFilterSubGroupViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.map_filters_sub_group_item_title);
            _recyclerView = itemView.FindViewById<MvxRecyclerView>(Resource.Id.map_filters_sub_group_item_recycle);
            var expandImage = itemView.FindViewById<ImageView>(Resource.Id.map_filters_sub_group_item_expand_image);
            var checkbox = itemView.FindViewById<MaterialCheckBox>(Resource.Id.map_filters_sub_group_item_checkbox);
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MapFilterSubGroupViewHolder, MapFilterSubGroupItemViewModel>();

                set.Bind(expandImage)
                    .For(v => v.Rotation)
                    .To(vm => vm.IsOpened)
                    .WithConversion(new AnyExpressionConverter<bool, float>(value => value ? 90f : 0));
                
                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(checkbox)
                    .For(v => v.Checked)
                    .To(vm => vm.IsSelected);
                
                set.Bind(_recyclerView)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsOpened);

                set.Apply();
            });
        }
        
        public override void Bind()
        {
            base.Bind();

            if (_filtersItemAdapter != null)
            {
                return;
            }

            _filtersItemAdapter = new MapFiltersItemListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.MapFiltersItemView),
            };

            _recyclerView.SetLayoutManager(new MvxGuardedLinearLayoutManager(_recyclerView.Context) { Orientation = LinearLayoutManager.Vertical });
            _recyclerView.SetAdapter(_filtersItemAdapter);

            var set = CreateBindingSet();

            set.Bind(_filtersItemAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Items);

            set.Apply();
        }
    }
    
    protected override void ExecuteCommandOnItem(ICommand command, object itemDataContext)
    {
        base.ExecuteCommandOnItem(command, itemDataContext);

        if (itemDataContext is MapFilterSubGroupItemViewModel viewModel)
        {
            viewModel.IsOpened = !viewModel.IsOpened;
        }
    }
}
