using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class CoursesListAdapter : BaseRecyclerViewAdapter<CourseItem, CoursesListAdapter.CourseViewHolder>
{
    public CoursesListAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, CourseViewHolder> BindableViewHolderCreator =>
        (v, c) => new CourseViewHolder(v, c);

    public class CourseViewHolder : BaseViewHolder
    {
        private readonly MvxRecyclerView _coursesList;

        private CourseTagsListAdapter _tagsListAdapter;

        public CourseViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.courses_expanded_list_item_title);
            _coursesList = itemView.FindViewById<MvxRecyclerView>(Resource.Id.courses_expanded_list_item_list);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            if (_tagsListAdapter != null)
            {
                return;
            }

            _tagsListAdapter = new CourseTagsListAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_tags_list_item),
            };

            _coursesList.SetLayoutManager(new MvxGuardedLinearLayoutManager(_coursesList.Context) { Orientation = LinearLayoutManager.Horizontal });
            _coursesList.SetAdapter(_tagsListAdapter);
            _coursesList.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(_coursesList.Context, 8), LinearLayoutManager.Horizontal));

            var set = CreateBindingSet();

            set.Bind(_tagsListAdapter)
                .For(x => x.ItemsSource)
                .To(vm => vm.Tags);

            set.Apply();
        }
    }
}