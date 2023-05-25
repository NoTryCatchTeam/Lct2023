using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Lct2023.Android.Decorations;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class CourseDetailsSectionAdapter : BaseRecyclerViewAdapter<CourseSectionItem, CourseDetailsSectionAdapter.SectionViewHolder>
{
    public CourseDetailsSectionAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, SectionViewHolder> BindableViewHolderCreator =>
        (v, c) => new SectionViewHolder(v, c);

    public class SectionViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _lessons;

        private CourseSectionLessonsAdapter _lessonsAdapter;

        public SectionViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.course_details_section_item_title);
            var progress = itemView.FindViewById<TextView>(Resource.Id.course_details_section_item_progress);
            var sync = itemView.FindViewById<MaterialButton>(Resource.Id.course_details_section_item_sync);
            _lessons = itemView.FindViewById<MvxRecyclerView>(Resource.Id.course_details_section_item_lessons);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Bind(progress)
                    .For(x => x.Text)
                    .To(vm => vm.Lessons)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<CourseLessonItem>, string>(
                        x => $"{x.Count(l => l.Status == CourseLessonStatus.Finished)}/{x.Count()} уроков пройдено"));

                set.Bind(sync)
                    .For(x => x.BindVisible())
                    .To(vm => vm.IsPossibleToSync);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            if (_lessonsAdapter != null)
            {
                return;
            }

            _lessonsAdapter = new CourseSectionLessonsAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.course_details_lesson_item),
            };

            _lessons.SetLayoutManager(new MvxGuardedLinearLayoutManager(_lessons.Context) { Orientation = LinearLayoutManager.Vertical });
            _lessons.SetAdapter(_lessonsAdapter);
            _lessons.AddItemDecoration(new ColoredDividerItemDecoration(_lessons.Context, LinearLayoutManager.Vertical)
                { Drawable = _lessons.Context.GetDrawable(Resource.Drawable.simple_list_item_decorator) });

            var set = CreateBindingSet();

            set.Bind(_lessonsAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Lessons);

            set.Apply();
        }
    }
}