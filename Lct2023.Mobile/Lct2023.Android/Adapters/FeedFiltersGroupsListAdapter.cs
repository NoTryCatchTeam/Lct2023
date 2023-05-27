using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Lct2023.ViewModels.Feed;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class FeedFiltersGroupsListAdapter : BaseRecyclerViewAdapter<FeedFilterGroupItemViewModel, FeedFiltersGroupsListAdapter.FeedFilterGroupViewHolder>
{
    public FeedFiltersGroupsListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, FeedFilterGroupViewHolder> BindableViewHolderCreator =>
        (v, c) => new FeedFilterGroupViewHolder(v, c);

    public class FeedFilterGroupViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _recyclerView;
        private MvxRecyclerAdapter _filtersItemAdapter;

        public FeedFilterGroupViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.feed_filters_group_item_title);
            _recyclerView = itemView.FindViewById<MvxRecyclerView>(Resource.Id.feed_filters_items_recycle);
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FeedFilterGroupViewHolder, FeedFilterGroupItemViewModel>();

                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);

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

            _filtersItemAdapter = new FiltersItemListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.FiltersItemView),
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
}
