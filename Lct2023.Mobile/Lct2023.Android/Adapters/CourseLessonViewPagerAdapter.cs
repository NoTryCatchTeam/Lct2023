using AndroidX.ConstraintLayout.Widget;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Lct2023.Android.Fragments.Courses;
using Lct2023.ViewModels.Courses;

namespace Lct2023.Android.Adapters;

public class CourseLessonViewPagerAdapter : FragmentStateAdapter
{
    private readonly Fragment[] _fragmentsArray;

    public CourseLessonViewPagerAdapter(FragmentActivity activity, CourseLessonItem lesson, CourseLessonViewModel courseLessonViewModel)
        : base(activity)
    {
        ItemCount = 3;

        _fragmentsArray = new Fragment[]
        {
            new LessonLessonPartFragment
            {
                DataContext = courseLessonViewModel,
            },
            new LessonTaskPartFragment
            {
                DataContext = lesson,
            },
            new LessonAnswerPartFragment
            {
                DataContext = courseLessonViewModel,
            },
        };
    }

    public override int ItemCount { get; }

    public override Fragment CreateFragment(int position) => _fragmentsArray[position];
}
