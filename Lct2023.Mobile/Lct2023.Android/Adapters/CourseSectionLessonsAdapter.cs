using System;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class CourseSectionLessonsAdapter : BaseRecyclerViewAdapter<CourseLessonItem, CourseSectionLessonsAdapter.LessonViewHolder>
{
    public CourseSectionLessonsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, LessonViewHolder> BindableViewHolderCreator =>
        (v, c) => new LessonViewHolder(v, c);

    public class LessonViewHolder : BaseViewHolder
    {
        public LessonViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var imageContainer = itemView.FindViewById<MaterialCardView>(Resource.Id.course_details_lesson_item_image_container);
            var image = itemView.FindViewById<ImageView>(Resource.Id.course_details_lesson_item_image);
            var title = itemView.FindViewById<TextView>(Resource.Id.course_details_lesson_item_title);
            var availability = itemView.FindViewById<TextView>(Resource.Id.course_details_lesson_item_availability);
            var lockImage = itemView.FindViewById<ImageView>(Resource.Id.course_details_lesson_item_lock);
            var reviewBadge = itemView.FindViewById<MaterialCardView>(Resource.Id.course_details_lesson_item_badge);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(imageContainer)
                    .For(nameof(CardViewStrokeColorBinding))
                    .To(vm => vm.Status)
                    .WithConversion(new AnyExpressionConverter<CourseLessonStatus, int>(x => x switch
                    {
                        CourseLessonStatus.Finished => Resource.Color.bgBorder,
                        _ => Resource.Color.courseLessonReadyBorder,
                    }));

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Bind(availability)
                    .For(x => x.Text)
                    .To(vm => vm.Status)
                    .WithConversion(new AnyExpressionConverter<CourseLessonStatus, string>(x => x switch
                    {
                        CourseLessonStatus.Available => "Уже доступно для просмотра",
                        CourseLessonStatus.Locked => "Завершите предыдущий урок",
                        CourseLessonStatus.Finished => "Завершен",
                        _ => null,
                    }));

                set.Bind(availability)
                    .For(x => x.BindVisible())
                    .To(vm => vm.Status)
                    .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x != CourseLessonStatus.WaitingForReview));

                set.Bind(reviewBadge)
                    .For(x => x.BindVisible())
                    .To(vm => vm.Status)
                    .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.WaitingForReview));

                set.Bind(lockImage)
                    .For(x => x.BindVisible())
                    .To(vm => vm.Status)
                    .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.Locked));

                set.Apply();
            });
        }
    }
}
