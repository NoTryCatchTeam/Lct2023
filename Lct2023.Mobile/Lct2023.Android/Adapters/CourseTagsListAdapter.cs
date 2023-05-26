using System;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Converters;
using Lct2023.Definitions.Types;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class CourseTagsListAdapter : BaseRecyclerViewAdapter<CourseTagItem, CourseTagsListAdapter.CourseTagViewHolder>
{
    public CourseTagsListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseTagViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseTagViewHolder(v, c);

    public class CourseTagViewHolder : BaseViewHolder
    {
        public CourseTagViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var layout = itemView.FindViewById<MaterialCardView>(Resource.Id.courses_tags_list_item_layout);
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_tags_list_item_title);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(layout)
                    .For(x => x.CardBackgroundColor)
                    .To(vm => vm.Type)
                    .WithConversion(new AnyExpressionConverter<CourseTagItemType, ColorStateList>(
                        x => itemView.Context.Resources.GetColorStateList(
                            x switch
                            {
                                CourseTagItemType.Free => Resource.Color.courseTagFree,
                                CourseTagItemType.Paid => Resource.Color.courseTagPaid,
                                CourseTagItemType.Lite => Resource.Color.courseTagDifficulty,
                                CourseTagItemType.Hard => Resource.Color.courseTagDifficulty,
                                _ => Resource.Color.courseTagOther,
                            },
                            null)));

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }
    }
}
