using System;
using Android.Views;
using Android.Widget;
using Google.Android.Material.CheckBox;
using Lct2023.ViewModels.Map.Filters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class MapFiltersItemListAdapter: BaseRecyclerViewAdapter<MapFilterItemViewModel, MapFiltersItemListAdapter.MapFilterItemViewHolder>
{
    public MapFiltersItemListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, MapFilterItemViewHolder> BindableViewHolderCreator =>
        (v, c) => new MapFilterItemViewHolder(v, c);

    public class MapFilterItemViewHolder : BaseViewHolder
    {
        public MapFilterItemViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.map_filters_item_title);
            var checkbox = itemView.FindViewById<MaterialCheckBox>(Resource.Id.map_filters_item_checkbox);
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MapFilterItemViewHolder, MapFilterItemViewModel>();

                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(checkbox)
                    .For(v => v.Checked)
                    .To(vm => vm.IsSelected);
                
                set.Apply();
            });
        }
    }
}
