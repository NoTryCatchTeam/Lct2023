using System;
using System.Windows.Input;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Map.Filters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class MapFiltersGroupsListAdapter : BaseRecyclerViewAdapter<MapFilterGroupItemViewModel, MapFiltersGroupsListAdapter.MapFilterGroupViewHolder>
{
    private Action _onExpandAction;

    public MapFiltersGroupsListAdapter(IMvxAndroidBindingContext bindingContext, Action onExpandAction)
        : base(bindingContext)
    {
        _onExpandAction = onExpandAction;
    }

    protected override Func<View, IMvxAndroidBindingContext, MapFilterGroupViewHolder> BindableViewHolderCreator =>
        (v, c) => new MapFilterGroupViewHolder(v, c, _onExpandAction);

    public class MapFilterGroupViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _recyclerView;
        private MvxRecyclerAdapter _filtersSubGroupAdapter;
        private Action _onExpandAction;

        public MapFilterGroupViewHolder(View itemView, IMvxAndroidBindingContext context, Action onExpandAction)
            : base(itemView, context)
        {
            _onExpandAction = onExpandAction;

            var title = itemView.FindViewById<TextView>(Resource.Id.map_filters_group_item_title);
            _recyclerView = itemView.FindViewById<MvxRecyclerView>(Resource.Id.map_filters_group_item_recycle);
            var expandImage = itemView.FindViewById<ImageView>(Resource.Id.map_filters_group_item_expand_image);
            
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MapFilterGroupViewHolder, MapFilterGroupItemViewModel>();

                set.Bind(expandImage)
                    .For(v => v.Rotation)
                    .To(vm => vm.IsOpened)
                    .WithConversion(new AnyExpressionConverter<bool, float>(value => value ? 90f : 0));
                
                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(_recyclerView)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsOpened);

                set.Apply();
            });
        }
        
        public override void Bind()
        {
            base.Bind();

            if (_filtersSubGroupAdapter != null)
            {
                return;
            }

            _filtersSubGroupAdapter = new MapFiltersSubGroupsListAdapter((IMvxAndroidBindingContext)BindingContext, _onExpandAction)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.MapFiltersSubGroupItemView),
            };

            _recyclerView.SetLayoutManager(new MvxGuardedLinearLayoutManager(_recyclerView.Context) { Orientation = LinearLayoutManager.Vertical });
            _recyclerView.SetAdapter(_filtersSubGroupAdapter);
            _recyclerView.AddItemDecoration(
                new ColoredDividerItemDecoration(_recyclerView.Context, LinearLayoutManager.Vertical)
                {
                    Drawable = _recyclerView.Context.GetDrawable(Resource.Drawable.simple_list_item_decorator),
                    Padding = new Rect(DimensUtils.DpToPx(_recyclerView.Context, 16), 0, DimensUtils.DpToPx(_recyclerView.Context, 16), 0),
                });

            var set = CreateBindingSet();

            set.Bind(_filtersSubGroupAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.SubGroups);

            set.Apply();
        }
    }
    
    protected override void ExecuteCommandOnItem(ICommand command, object itemDataContext)
    {
        base.ExecuteCommandOnItem(command, itemDataContext);

        if (itemDataContext is MapFilterGroupItemViewModel viewModel)
        {
            viewModel.IsOpened = !viewModel.IsOpened;
            _onExpandAction();
        }
    }
}
