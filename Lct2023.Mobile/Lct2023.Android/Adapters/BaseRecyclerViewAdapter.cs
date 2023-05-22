using System;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public abstract class BaseRecyclerViewAdapter<TItem, TViewHolder> : MvxRecyclerAdapter
    where TViewHolder : BaseRecyclerViewAdapter<TItem, TViewHolder>.BaseViewHolder
{
    protected BaseRecyclerViewAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected abstract Func<View, IMvxAndroidBindingContext, TViewHolder> BindableViewHolderCreator { get; }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var bindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

        return BindableViewHolderCreator(
            InflateViewForHolder(parent, viewType, bindingContext),
            bindingContext);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        base.OnBindViewHolder(holder, position);

        ((BaseViewHolder)holder).Bind();
    }

    public abstract class BaseViewHolder : MvxRecyclerViewHolder
    {
        protected BaseViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }

        protected TItem ViewModel => (TItem)DataContext;

        public virtual void Bind()
        {
        }

        protected MvxFluentBindingDescriptionSet<BaseViewHolder, TItem> CreateBindingSet() =>
            this.CreateBindingSet<BaseViewHolder, TItem>();
    }
}
