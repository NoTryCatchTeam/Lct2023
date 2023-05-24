using Android.Animation;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Android.TemplateSelectors;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class TasksFragment : BaseFragment<TasksViewModel>
{
    private (Animator Show, Animator Hide) _scrollAnimators;

    protected override int GetLayoutId() => Resource.Layout.TasksFragment;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var parent = view.FindViewById<ConstraintLayout>(Resource.Id.tasks_layout);
        var searchLayout = view.FindViewById<TextInputLayout>(Resource.Id.tasks_search_layout);
        var search = view.FindViewById<TextInputEditText>(Resource.Id.tasks_search_value);
        var scroll = view.FindViewById<NestedScrollView>(Resource.Id.tasks_scroll);

        var stats = new StatsViews(
            new StatsViews.Stat(view.FindViewById<TextView>(Resource.Id.tasks_stats_exercises_counter), view.FindViewById<LinearProgressIndicator>(Resource.Id.tasks_stats_exercises_progress)),
            new StatsViews.Stat(view.FindViewById<TextView>(Resource.Id.tasks_stats_tasks_counter), view.FindViewById<LinearProgressIndicator>(Resource.Id.tasks_stats_tasks_progress)),
            new StatsViews.Stat(view.FindViewById<TextView>(Resource.Id.tasks_stats_points_counter), view.FindViewById<LinearProgressIndicator>(Resource.Id.tasks_stats_points_progress))
        );

        var taskOfTheDay = view.FindViewById(Resource.Id.tasks_task_otd);
        var taskOfTheDayLoader = view.FindViewById(Resource.Id.tasks_task_otd_loader);
        var tasksFilter = view.FindViewById<MaterialButton>(Resource.Id.tasks_tasks_filter);
        var tasksList = view.FindViewById<MvxRecyclerView>(Resource.Id.tasks_tasks_list);

        scroll.SetOnScrollChangeListener(new DefaultScrollListener((_, _, scrollY, _, oldScrollY) =>
        {
            const int SCROLL_DIFF_THRESHOLD = 3;

            var isScrollDown = scrollY - oldScrollY > 0;
            var isScrollUp = scrollY - oldScrollY < 0;

            if (isScrollDown && scrollY - oldScrollY > SCROLL_DIFF_THRESHOLD && !_scrollAnimators.Hide.IsRunning && searchLayout.TranslationY == 0)
            {
                _scrollAnimators.Show.Cancel();
                _scrollAnimators.Hide.Start();
            }
            else if (isScrollUp && (scrollY - oldScrollY < -SCROLL_DIFF_THRESHOLD || scrollY < scroll.PaddingTop / 2) && !_scrollAnimators.Show.IsRunning && searchLayout.TranslationY < 0)
            {
                _scrollAnimators.Hide.Cancel();
                _scrollAnimators.Show.Start();
            }
        }));

        var tasksAdapter = new TasksAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MultipleTemplateSelector<TaskItem>(
                item => item.IsCompleted ? 1 : 0,
                type => type == 1 ? Resource.Layout.tasks_list_item_active : Resource.Layout.tasks_list_item),
        };

        tasksList.SetLayoutManager(new MvxGuardedGridLayoutManager(Activity, 3) { Orientation = LinearLayoutManager.Vertical });
        tasksList.SetAdapter(tasksAdapter);
        tasksList.AddItemDecoration(new GridItemDecoration(3, DimensUtils.DpToPx(Activity, 16)));

        parent.Post(() =>
        {
            var searchRect = new Rect();
            searchLayout.GetDrawingRect(searchRect);
            parent.OffsetDescendantRectToMyCoords(searchLayout, searchRect);

            scroll.SetPadding(scroll.PaddingLeft, scroll.PaddingTop + searchRect.Height(), scroll.PaddingRight, scroll.PaddingBottom);

            _scrollAnimators = (
                ObjectAnimator.OfFloat(searchLayout, nameof(View.TranslationY), -searchRect.Height(), 0)
                    .SetDuration(200),
                ObjectAnimator.OfFloat(searchLayout, nameof(View.TranslationY), 0, -searchRect.Height())
                    .SetDuration(200));
        });

        var set = CreateBindingSet();

        set.Bind(taskOfTheDay)
            .For(x => x.BindClick())
            .To(vm => vm.TaskOfTheDayCommand);

        set.Bind(taskOfTheDayLoader)
            .For(x => x.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<TasksViewState, bool>(x => x.HasFlag(TasksViewState.TaskOfTheDayLoading)));

        set.Bind(tasksFilter)
            .For(x => x.BindClick())
            .To(vm => vm.TasksFilterCommand);

        set.Bind(tasksAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.TasksCollection);

        set.Bind(tasksAdapter)
            .For(x => x.ItemClick)
            .To(vm => vm.TaskTapCommand);

        set.Apply();

        return view;
    }

    private class StatsViews
    {
        public StatsViews(Stat exercises, Stat tasks, Stat points)
        {
            Exercises = exercises;
            Tasks = tasks;
            Points = points;
        }

        public Stat Exercises { get; }

        public Stat Tasks { get; }

        public Stat Points { get; }

        public class Stat
        {
            public Stat(TextView counter, LinearProgressIndicator progress)
            {
                Counter = counter;
                Progress = progress;
            }

            public TextView Counter { get; }

            public LinearProgressIndicator Progress { get; }
        }
    }
}
