using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Adapters;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Android.Presenters;
using Lct2023.Android.Views;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Binding;

namespace Lct2023.Android.Activities;

[MvxRootActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class MainTabbedActivity : BaseActivity<MainTabbedViewModel>
{
    private readonly Dictionary<int, int> _tabMenuActionsToFragmentPositionsMap;

    private ConstraintLayout _parent;
    private ViewPager2 _viewPager;
    private BottomNavigationView _bottomNavigationView;

    public MainTabbedActivity()
    {
        _tabMenuActionsToFragmentPositionsMap = new Dictionary<int, int>
        {
            { Resource.Id.action_main, 0 },
            { Resource.Id.action_courses, 1 },
            { Resource.Id.action_feed, 2 },
            { Resource.Id.action_tasks, 3 },
            { Resource.Id.action_map, 4 },
        };
    }

    public void NavigateToPosition(int index)
    {
        _viewPager.SetCurrentItem(index, false);
        _bottomNavigationView.SelectedItemId = _tabMenuActionsToFragmentPositionsMap.First(x => x.Value == index).Key;
    }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.MainTabbedActivity);

        _parent = FindViewById<ConstraintLayout>(Resource.Id.main_view_layout);
        _viewPager = FindViewById<ViewPager2>(Resource.Id.main_view_pager);
        _bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.main_view_bottom_navigation);

        var onboardingViews = new OnboardingViews(
            FindViewById<MaskView>(Resource.Id.onboarding_mask),
            FindViewById<MotionLayout>(Resource.Id.onboarding_motion_layout),
            FindViewById<ImageView>(Resource.Id.onboarding_info_arrow),
            FindViewById<MaterialCardView>(Resource.Id.onboarding_info),
            FindViewById<TextView>(Resource.Id.onboarding_info_text),
            FindViewById<MaterialButton>(Resource.Id.onboarding_info_next),
            FindViewById<MaterialButton>(Resource.Id.onboarding_info_skip),
            FindViewById<TextView>(Resource.Id.onboarding_info_counter));

        _viewPager.UserInputEnabled = false;
        _viewPager.Adapter = new MainViewPagerAdapter(this, 5);
        _viewPager.OffscreenPageLimit = _viewPager.Adapter.ItemCount;

        _bottomNavigationView.SetOnItemSelectedListener(new DefaultNavBarItemSelectedListener(
            item =>
            {
                _viewPager.SetCurrentItem(_tabMenuActionsToFragmentPositionsMap[item.ItemId], false);

                return true;
            }));

        onboardingViews.Mask.Visibility = ViewStates.Gone;
        onboardingViews.MotionLayout.Visibility = ViewStates.Gone;

        if (!Xamarin.Essentials.VersionTracking.IsFirstLaunchEver)
        {
            return;
        }

        // Onboarding setup
        onboardingViews.Mask.Visibility = ViewStates.Visible;
        onboardingViews.MotionLayout.Visibility = ViewStates.Visible;

        onboardingViews.Mask.Clickable = true;

        onboardingViews.Counter.TextFormatted = GetCounterString(1);

        var motionConstraintSets = new (int Id, ConstraintSet ConstraintSet)[]
        {
            (Resource.Id.onboarding_scene_step_1, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_1)),
            (Resource.Id.onboarding_scene_step_2, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_2)),
            (Resource.Id.onboarding_scene_step_3, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_3)),
            (Resource.Id.onboarding_scene_step_4, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_4)),
            (Resource.Id.onboarding_scene_step_5, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_5)),
            (Resource.Id.onboarding_scene_step_6, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_6)),
            (Resource.Id.onboarding_scene_step_7, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_7)),
            (Resource.Id.onboarding_scene_step_8, onboardingViews.MotionLayout.GetConstraintSet(Resource.Id.onboarding_scene_step_8)),
        };

        var maskViews = new (View View, Rect Rect)[8];

        var stepCounter = 1;
        var clickListener = new DefaultClickListener(v =>
        {
            switch (v.Id)
            {
                case Resource.Id.onboarding_info_next:

                    if (stepCounter == 8)
                    {
                        // Finish onboarding
                        onboardingViews.Mask.Visibility = ViewStates.Gone;
                        onboardingViews.MotionLayout.Visibility = ViewStates.Gone;

                        ViewModel.FinishOnboardingCommand.ExecuteAsync();

                        return;
                    }

                    onboardingViews.Mask.AnimateToRect(maskViews[stepCounter].Rect, 300);
                    onboardingViews.MotionLayout.TransitionToState(stepCounter switch
                    {
                        1 => Resource.Id.onboarding_scene_step_2,
                        2 => Resource.Id.onboarding_scene_step_3,
                        3 => Resource.Id.onboarding_scene_step_4,
                        4 => Resource.Id.onboarding_scene_step_5,
                        5 => Resource.Id.onboarding_scene_step_6,
                        6 => Resource.Id.onboarding_scene_step_7,
                        7 => Resource.Id.onboarding_scene_step_8,
                    });

                    onboardingViews.InfoText.Text = stepCounter switch
                    {
                        1 => "Полноценные видеокурсы для обучения по направлениям, форматам и стоимости, которые больше всего подходят для вас.",
                        2 => "Лента новостей, в которой вы сможете найти интересный и полезный контент из мира музыки, театра и хореографии.",
                        3 => "Проходите ежедневные тесты и зарабатывайте баллы рейтинга, чтобы обойти своих друзей в соревновании.",
                        4 => "Карта со школами МШИ и мероприятиями города Москва: театры, концерты, выставки.",
                        5 => "Ваш профиль, который доступен из любого раздела. В нем вы можете посмотреть статистику и рейтинг.",
                        6 => "Каждый день новые сторис на главной. Получайте полезную информацию, вовлекайтесь и проходите ежедневные задания.",
                        7 => "Карта со школами МШИ и мероприятиями города Москва: театры, концерты, выставки.",
                    };

                    onboardingViews.Counter.TextFormatted = GetCounterString(stepCounter);

                    stepCounter += 1;

                    if (stepCounter == 8)
                    {
                        onboardingViews.Next.Text = "Начать пользоваться!";
                        onboardingViews.Skip.Visibility = ViewStates.Gone;
                    }

                    break;
                case Resource.Id.onboarding_info_skip:
                    onboardingViews.Mask.Visibility = ViewStates.Gone;
                    onboardingViews.MotionLayout.Visibility = ViewStates.Gone;

                    break;
            }
        });

        onboardingViews.Next.SetOnClickListener(clickListener);
        onboardingViews.Skip.SetOnClickListener(clickListener);

        _parent.Post(() =>
        {
            var viewPagerRecyclerViewLayoutManager = ((RecyclerView)_viewPager.GetChildAt(0)).GetLayoutManager().FindViewByPosition(0);

            maskViews[0] = GetViewInfo(FindViewById(Resource.Id.action_main));
            maskViews[1] = GetViewInfo(FindViewById(Resource.Id.action_courses));
            maskViews[2] = GetViewInfo(FindViewById(Resource.Id.action_feed));
            maskViews[3] = GetViewInfo(FindViewById(Resource.Id.action_tasks));
            maskViews[4] = GetViewInfo(FindViewById(Resource.Id.action_map));
            maskViews[5] = GetViewInfo(viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.toolbar_content));
            maskViews[6] = GetViewInfo(viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.main_stories));
            maskViews[7] = GetViewInfo(viewPagerRecyclerViewLayoutManager.FindViewById(Resource.Id.main_statistics_frame));

            for (var i = 0; i < maskViews.Length; i++)
            {
                var maskView = maskViews[i];

                if (i != 5)
                {
                    maskView.Rect.Inset(DimensUtils.DpToPx(this, 4), DimensUtils.DpToPx(this, 4));
                }

                var constraintSet = motionConstraintSets[i];

                var translationY = i < 5 ?
                    -1 * (_parent.MeasuredHeight - maskView.Rect.Top + DimensUtils.DpToPx(this, 12)) :
                    maskView.Rect.Bottom + DimensUtils.DpToPx(this, 12);

                var arrowMargin = maskView.Rect.CenterX() - onboardingViews.Triangle.MeasuredWidth / 2;

                constraintSet.ConstraintSet.SetTranslationY(Resource.Id.onboarding_info, translationY);
                constraintSet.ConstraintSet.SetTranslationY(Resource.Id.onboarding_info_arrow, translationY);
                constraintSet.ConstraintSet.SetMargin(Resource.Id.onboarding_info_arrow, ConstraintSet.Start, arrowMargin);

                onboardingViews.MotionLayout.UpdateState(constraintSet.Id, constraintSet.ConstraintSet);
            }

            onboardingViews.MotionLayout.RebuildScene();

            onboardingViews.Mask.SetInitialRect(maskViews[0].Rect);

            (View View, Rect Rect) GetViewInfo(View view)
            {
                var viewRect = new Rect();
                view.GetDrawingRect(viewRect);
                _parent.OffsetDescendantRectToMyCoords(view, viewRect);

                return (view, viewRect);
            }
        });
    }

    private SpannableStringBuilder GetCounterString(int stepCounter)
    {
        var stepNumber = (stepCounter + 1).ToString();
        var counterText = new SpannableStringBuilder(stepNumber);

        counterText.SetSpan(
            new ForegroundColorSpan(Resources.GetColor(Resource.Color.accent, null)),
            0,
            stepNumber.Length,
            SpanTypes.ExclusiveExclusive);

        counterText.Append("/8");

        return counterText;
    }

    private class OnboardingViews
    {
        public OnboardingViews(
            MaskView mask,
            MotionLayout motionLayout,
            ImageView triangle,
            MaterialCardView infoView,
            TextView infoText,
            MaterialButton next,
            MaterialButton skip,
            TextView counter)
        {
            Mask = mask;
            MotionLayout = motionLayout;
            Triangle = triangle;
            InfoView = infoView;
            InfoText = infoText;
            Next = next;
            Skip = skip;
            Counter = counter;
        }

        public MaskView Mask { get; }

        public MotionLayout MotionLayout { get; }

        public ImageView Triangle { get; }

        public MaterialCardView InfoView { get; }

        public TextView InfoText { get; }

        public MaterialButton Next { get; }

        public MaterialButton Skip { get; }

        public TextView Counter { get; }
    }
}
