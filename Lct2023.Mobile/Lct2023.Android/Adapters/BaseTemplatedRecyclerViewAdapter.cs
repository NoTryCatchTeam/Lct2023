using System;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public abstract class BaseTemplatedRecyclerViewAdapter<TItem, TViewHolder> : BaseRecyclerViewAdapter<TItem, TViewHolder>
    where TViewHolder : BaseRecyclerViewAdapter<TItem, TViewHolder>.BaseViewHolder
{
    public BaseTemplatedRecyclerViewAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, TViewHolder> BindableViewHolderCreator { get; }
    
    protected abstract Func<View, int, IMvxAndroidBindingContext, TViewHolder> BindableTemplatedViewHolderCreator { get; }
    
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var bindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

        return BindableTemplatedViewHolderCreator(
            InflateViewForHolder(parent, viewType, bindingContext),
            viewType,
            bindingContext);
    }
}