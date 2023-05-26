using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.Tabs;
using Lct2023.Android.Adapters;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Activities.Courses;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class CourseLessonActivity : BaseActivity<CourseLessonViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.CourseLessonActivity);

        var toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar_inner);
        toolbar.Title = "Гитара";
        toolbar.SetOnMenuItemClickListener(new DefaultMenuItemClickListener(_ =>
        {
            ViewModel.NavigateBackCommand.ExecuteAsync();

            return true;
        }));

        var info = new Info(
            FindViewById<ImageView>(Resource.Id.course_lesson_info_image),
            FindViewById<TextView>(Resource.Id.course_lesson_info_title),
            FindViewById<TextView>(Resource.Id.course_lesson_info_description),
            FindViewById<TextView>(Resource.Id.course_lesson_info_section_text),
            FindViewById<TextView>(Resource.Id.course_lesson_info_lesson_text));

        var tabs = FindViewById<TabLayout>(Resource.Id.course_lesson_tabs);
        var viewPager = FindViewById<ViewPager2>(Resource.Id.course_lesson_view_pager);

        viewPager.UserInputEnabled = false;
        viewPager.OffscreenPageLimit = 2;
        viewPager.Adapter = new CourseLessonViewPagerAdapter(this,  ViewModel.NavigationParameter.LessonItem, ViewModel);

        new TabLayoutMediator(
                tabs,
                viewPager,
                false,
                true,
                new DefaultTabConfigurationStrategy(
                    (tab, position) =>
                    {
                        tab.SetText(position switch
                        {
                            0 => "Урок",
                            1 => "Задание",
                            2 => "Ответы",
                            _ => string.Empty,
                        });
                    }))
            .Attach();

        var set = CreateBindingSet();

        set.Bind(info.Title)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Title);

        set.Bind(info.Description)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Description);

        set.Bind(info.Section)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.SectionNumber)
            .WithConversion(new AnyExpressionConverter<int, string>(x => $"Глава {x}"));

        set.Bind(info.Lesson)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Number)
            .WithConversion(new AnyExpressionConverter<int, string>(x => $"Урок {x}"));

        set.Apply();
    }

    private class Info
    {
        public Info(ImageView image, TextView title, TextView description, TextView section, TextView lesson)
        {
            Image = image;
            Title = title;
            Description = description;
            Section = section;
            Lesson = lesson;
        }

        public ImageView Image { get; }

        public TextView Title { get; }

        public TextView Description { get; }

        public TextView Section { get; }

        public TextView Lesson { get; }
    }
}
