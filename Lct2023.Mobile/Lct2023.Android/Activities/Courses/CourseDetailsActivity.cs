using System;
using System.Reactive.Disposables;
using Android.Animation;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Airbnb.Lottie;
using DynamicData.Binding;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Java.Lang;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Activities.Courses;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public partial class CourseDetailsActivity : BaseActivity<CourseDetailsViewModel>, Animator.IAnimatorListener
{
    private (ConstraintLayout Container, LottieAnimationView Animation) _lottie;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.CourseDetailsActivity);

        Toolbar.Title = "Гитара";

        var views = new Views(
            new Views.TeacherViews(
                FindViewById<ImageView>(Resource.Id.course_details_teacher_image),
                FindViewById<TextView>(Resource.Id.course_details_teacher_name),
                FindViewById<TextView>(Resource.Id.course_details_teacher_description)),
            new Views.InfoViews(
                FindViewById<MaterialCardView>(Resource.Id.course_details_info),
                new Views.InfoViews.ExtenderViews(
                    FindViewById<ConstraintLayout>(Resource.Id.course_details_info_extender),
                    FindViewById<ImageView>(Resource.Id.course_details_info_extender_chevron)),
                new Views.InfoViews.DetailsViews(
                    FindViewById<ConstraintLayout>(Resource.Id.course_details_info_extended_container),
                    FindViewById<MvxRecyclerView>(Resource.Id.course_details_info_tags),
                    FindViewById<MaterialButton>(Resource.Id.course_details_info_map),
                    FindViewById<TextView>(Resource.Id.course_details_info_full_price_value),
                    FindViewById<TextView>(Resource.Id.course_details_info_credit_price_value))),
            new Views.ProgressViews(
                FindViewById<MaterialCardView>(Resource.Id.course_details_progress),
                FindViewById<TextView>(Resource.Id.course_details_progress_value)),
            FindViewById<MvxRecyclerView>(Resource.Id.course_details_sections_list),
            FindViewById<MaterialButton>(Resource.Id.course_details_purchase)
        );

        _lottie = (
            FindViewById<ConstraintLayout>(Resource.Id.course_details_purchase_animation_container),
            FindViewById<LottieAnimationView>(Resource.Id.course_details_purchase_animation));

        var tagsAdapter = new CourseTagsListAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.courses_tags_list_item),
        };

        views.Info.Details.Tags.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
        views.Info.Details.Tags.SetAdapter(tagsAdapter);
        views.Info.Details.Tags.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(this, 8), LinearLayoutManager.Horizontal));

        var sectionsAdapter = new CourseDetailsSectionAdapter((IMvxAndroidBindingContext)BindingContext, ViewModel.LessonTapCommand)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.course_details_section_item),
        };

        views.Sections.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Vertical });
        views.Sections.SetAdapter(sectionsAdapter);
        views.Sections.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(this, 32), LinearLayoutManager.Vertical));
        views.Sections.HasFixedSize = false;

        views.Purchase.SetOnClickListener(new DefaultClickListener(_ =>
        {
            _lottie.Container.Animate()
                .Alpha(1)
                .WithStartAction(new Runnable(() => _lottie.Container.Visibility = ViewStates.Visible))
                .WithEndAction(new Runnable(() => _lottie.Animation.PlayAnimation()))
                .SetDuration(200)
                .Start();

            ViewModel.NavigationParameter.CourseItem.IsPurchased = true;
        }));

        _lottie.Container.Alpha = 0;
        _lottie.Animation.AddAnimatorListener(this);

        var set = CreateBindingSet();

        set.Bind(views.Info.Extender.Layout)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.CourseItem.IsPurchased);

        set.Bind(views.Info.Details.Layout)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.CourseItem.IsPurchased)
            .WithConversion(new AnyExpressionConverter<bool, bool>(x => !x));

        set.Bind(tagsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.CourseTagsCollection);

        set.Bind(sectionsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.CourseSectionsCollection);

        set.Bind(views.Purchase)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.CourseItem.IsPurchased)
            .WithConversion(new AnyExpressionConverter<bool, bool>(x => !x));

        set.Apply();

        ViewModel.NavigationParameter.CourseItem.WhenValueChanged(x => x.IsPurchased)
            .Subscribe(x =>
            {
                // Do not need to remove listener as course couldn't be refunded now
                if (!x)
                {
                    return;
                }

                views.Info.Layout.SetOnClickListener(new DefaultClickListener(_ =>
                {
                    var shouldShow = views.Info.Details.Layout.Visibility == ViewStates.Gone;

                    views.Info.Details.Layout.Visibility = shouldShow ? ViewStates.Visible : ViewStates.Gone;
                    views.Info.Extender.Chevron.Rotation = shouldShow ? 90 : 0;
                }));
            })
            .DisposeWith(CompositeDisposable);
    }

    public void OnAnimationCancel(Animator animation)
    {
    }

    public void OnAnimationEnd(Animator animation)
    {
        _lottie.Container.Animate()
            .Alpha(0)
            .WithEndAction(new Runnable(() => _lottie.Container.Visibility = ViewStates.Gone))
            .SetDuration(200)
            .Start();
    }

    public void OnAnimationRepeat(Animator animation)
    {
    }

    public void OnAnimationStart(Animator animation)
    {
        try
        {
            MediaPlayer.Create(this, Uri.Parse($"android.resource://{PackageName}/{Resource.Raw.achievement_bell}"))
                .Start();
        }
        catch
        {
            // ignored
        }
    }
}
