using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Adapters;

public class CoursesGroupsListAdapter : BaseRecyclerViewAdapter<CourseGroupItem, CoursesGroupsListAdapter.CourseGroupViewHolder>
{
    private readonly IMvxCommand<CourseItem> _courseTapCommand;

    public CoursesGroupsListAdapter(IMvxAndroidBindingContext bindingContext, IMvxCommand<CourseItem> courseTapCommand)
        : base(bindingContext)
    {
        _courseTapCommand = courseTapCommand;
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseGroupViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseGroupViewHolder(v, c);

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        base.OnBindViewHolder(holder, position);

        ((CourseGroupViewHolder)holder).SetCourseTapCommand(_courseTapCommand);
    }

    public class CourseGroupViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _coursesList;
        private readonly ImageView _chevron;

        private CoursesListAdapter _coursesAdapter;

        public CourseGroupViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var image = itemView.FindViewById<ImageView>(Resource.Id.courses_list_item_image);
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_list_item_title);
            var availability = itemView.FindViewById<TextView>(Resource.Id.courses_list_item_availability);
            _chevron = itemView.FindViewById<ImageView>(Resource.Id.courses_list_item_chevron);
            _coursesList = itemView.FindViewById<MvxRecyclerView>(Resource.Id.courses_list_item_list);
            _coursesList.Visibility = ViewStates.Gone;

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(image)
                    .For(nameof(MvxImageViewResourceNameTargetBinding))
                    .To(vm => vm.MajorType)
                    .WithConversion(new AnyExpressionConverter<CourseMajorType, int>(x => x switch
                    {
                        CourseMajorType.Guitar => Resource.Drawable.image_course_guitar,
                        CourseMajorType.FrenchHorn => Resource.Drawable.image_course_french_horn,
                        CourseMajorType.Drums => Resource.Drawable.image_course_drums,
                        _ => Resource.Drawable.image_course_guitar,
                    }));

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Major);

                set.Bind(availability)
                    .For(x => x.Text)
                    .To(vm => vm.Courses)
                    .WithConversion(new AnyExpressionConverter<IEnumerable<CourseItem>, string>(x => $"Доступно {x.Count()} курса"));

                set.Apply();
            });
        }

        public void SetCourseTapCommand(IMvxCommand<CourseItem> command)
        {
            if (_coursesAdapter.ItemClick != null)
            {
                return;
            }

            _coursesAdapter.ItemClick = command;
        }

        public override void Bind()
        {
            base.Bind();

            if (_coursesAdapter != null)
            {
                return;
            }

            _coursesAdapter = new CoursesListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_expanded_list_item),
            };

            _coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(_coursesList.Context) { Orientation = LinearLayoutManager.Vertical });
            _coursesList.SetAdapter(_coursesAdapter);
            _coursesList.AddItemDecoration(
                new ColoredDividerItemDecoration(_coursesList.Context, LinearLayoutManager.Vertical)
                {
                    Drawable = _coursesList.Context.GetDrawable(Resource.Drawable.simple_list_item_decorator),
                    Padding = new Rect(DimensUtils.DpToPx(_coursesList.Context, 16), 0, DimensUtils.DpToPx(_coursesList.Context, 16), 0),
                });

            var set = CreateBindingSet();

            set.Bind(_coursesAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Courses);

            set.Apply();
        }

        protected override void OnItemViewClick(object sender, EventArgs e)
        {
            base.OnItemViewClick(sender, e);

            var shouldOpen = _coursesList.Visibility == ViewStates.Gone;

            _coursesList.Visibility = shouldOpen ? ViewStates.Visible : ViewStates.Gone;
            _chevron.Rotation = shouldOpen ? 90 : 0;
        }
    }
}
