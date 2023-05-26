using System;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Courses;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class CoursesBannersAdapter : BaseRecyclerViewAdapter<BannerItem, CoursesBannersAdapter.BannerViewHolder>
{
    public CoursesBannersAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, BannerViewHolder> BindableViewHolderCreator =>
        (v, c) => new BannerViewHolder(v, c);

    public class BannerViewHolder : BaseViewHolder
    {
        public BannerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var text = itemView.FindViewById<TextView>(Resource.Id.courses_banner_item_text);
            var image = itemView.FindViewById<ImageView>(Resource.Id.courses_banner_item_image);
        }
    }
}
