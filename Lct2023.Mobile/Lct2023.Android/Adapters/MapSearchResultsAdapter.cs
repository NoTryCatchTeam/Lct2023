using System;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class MapSearchResultsAdapter : BaseTemplatedRecyclerViewAdapter<MapSearchResultItemViewModel, MapSearchResultsAdapter.MapSearchResultViewHolder>
{
    public MapSearchResultsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, int, IMvxAndroidBindingContext, MapSearchResultViewHolder> BindableTemplatedViewHolderCreator =>
        (v, t,  c) => t switch
        {
            Resource.Layout.SchoolSearchResultItemView => new SchoolSearchResultViewHolder(v, c),
            Resource.Layout.EventSearchResultItemView => new EventSearchResultViewHolder(v, c),
        };
    
    public abstract class MapSearchResultViewHolder : BaseViewHolder
    {
        protected MapSearchResultViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.map_search_result_item_title);
            var address = itemView.FindViewById<TextView>(Resource.Id.map_search_result_item_address);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MapSearchResultViewHolder, MapSearchResultItemViewModel>();
                
                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(address)
                    .For(v => v.Text)
                    .To(vm => vm.Address);

                set.Apply();
            });
        }
    }

    public class SchoolSearchResultViewHolder : MapSearchResultViewHolder
    {
        public SchoolSearchResultViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var card = itemView.FindViewById<MaterialCardView>(Resource.Id.map_search_result_item_image_background);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SchoolSearchResultViewHolder, MapSearchResultItemViewModel>();

                set.Bind(card)
                    .For(nameof(CardViewBackgroundColorByHexBinding))
                    .To(vm => vm.HexColor);
                
                set.Apply();
            });
        }
    }
    
    public class EventSearchResultViewHolder : MapSearchResultViewHolder
    {
        public EventSearchResultViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }
    }
}
