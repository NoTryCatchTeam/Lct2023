using System;
using System.Reactive.Disposables;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using DynamicData.Binding;
using Google.Android.Material.Button;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Fragments;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using ReactiveUI;

namespace Lct2023.Android.Activities.Tasks;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class TaskDetailsActivity : BaseActivity<TaskDetailsViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.TaskDetailsActivity);

        var title = FindViewById<TextView>(Resource.Id.task_details_title);
        var close = FindViewById<TextView>(Resource.Id.task_details_close);
        var counter = FindViewById<MvxRecyclerView>(Resource.Id.task_details_counter);
        var viewPager = FindViewById<ViewPager2>(Resource.Id.task_details_view_pager);
        var ctaButton = FindViewById<MaterialButton>(Resource.Id.task_details_button);

        var counterAdapter = new TaskDetailsCounterAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.task_details_progress_item),
        };

        counter.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
        counter.SetAdapter(counterAdapter);
        counter.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(this, 6), LinearLayoutManager.Horizontal));
        counter.AddOnItemTouchListener(new ExtendedSimpleOnItemTouchListener());

        viewPager.Adapter = new TaskDetailsViewPagerAdapter(this, ViewModel.ExercisesCollection);
        viewPager.UserInputEnabled = false;
        viewPager.OffscreenPageLimit = 1;

        var set = CreateBindingSet();

        set.Bind(title)
            .For(x => x.Text)
            .To(vm => vm.CurrentExercise.Number)
            .WithConversion(new AnyExpressionConverter<int, string>(x => $"Задание #{x}"));

        set.Bind(close)
            .For(x => x.BindClick())
            .To(vm => vm.NavigateBackCommand);

        set.Bind(counterAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.ExercisesCollection);

        set.Bind(ctaButton)
            .For(x => x.BindClick())
            .To(vm => vm.CallToActionCommand);

        set.Apply();

        ViewModel.WhenValueChanged(x => x.CurrentExercise, false)
            .Subscribe(x =>
            {
                if (SupportFragmentManager.FindFragmentByTag($"f{viewPager.CurrentItem}") is IPlayersFragment playerFragment)
                {
                    playerFragment.ReleasePlayers();
                }

                viewPager.SetCurrentItem(x.Number - 1, true);
            })
            .DisposeWith(CompositeDisposable);

        ViewModel.WhenAnyValue(
                x => x.IsPreSelectedAnswer,
                x => x.CurrentExercise.IsCorrect)
            .Subscribe(((bool IsPreSelected, bool? IsCorrect) x) =>
            {
                ctaButton.Visibility = x.IsPreSelected || x.IsCorrect != null ? ViewStates.Visible : ViewStates.Gone;

                ctaButton.StrokeWidth = x.IsPreSelected ? DimensUtils.DpToPx(this, 1) : 0;
                ctaButton.Text = x.IsPreSelected ? "Выбрать" : "Следующий вопрос";
                ctaButton.SetTextColor(GetColorStateList(x.IsPreSelected ? Resource.Color.textLink : Resource.Color.textLight));
                ctaButton.BackgroundTintList = GetColorStateList(x.IsPreSelected ? Resource.Color.background : Resource.Color.lightPurple);
            })
            .DisposeWith(CompositeDisposable);
    }

    private class ExtendedSimpleOnItemTouchListener : RecyclerView.SimpleOnItemTouchListener
    {
        public override bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e) => true;
    }
}
