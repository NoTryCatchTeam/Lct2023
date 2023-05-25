using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Lct2023.Helpers;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CoursesViewModel : BaseViewModel
{
    public CoursesViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        CourseTapCommand = new MvxAsyncCommand<CourseItem>(CourseTapAsync);

        BannersCollection = new MvxObservableCollection<BannerItem>
        {
            new (),
            new (),
            new (),
            new (),
            new (),
            new (),
        };

        CoursesGroupsCollection = new MvxObservableCollection<CourseGroupItem>
        {
            new (
                CourseMajorType.Guitar,
                new[]
                {
                    new CourseItem(
                        "Гитара для начинающих",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Видеокурс", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Гитара для ПРО",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
                            new CourseTagItem("Hard", CourseTagItemType.Hard),
                        }),
                    new CourseItem(
                        "Концерт на гитаре",
                        new[]
                        {
                            new CourseTagItem("Бесплатно", CourseTagItemType.Free),
                            new CourseTagItem("Видео", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Как устроена гитара",
                        new[]
                        {
                            new CourseTagItem("Бесплатно", CourseTagItemType.Free),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                }),
            new (
                CourseMajorType.FrenchHorn,
                new[]
                {
                    new CourseItem(
                        "Что такое валторна?",
                        new[]
                        {
                            new CourseTagItem("Бесплатно", CourseTagItemType.Paid),
                            new CourseTagItem("Видеокурс", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Первый урок",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
                            new CourseTagItem("Hard", CourseTagItemType.Hard),
                        }),
                }),
            new (
                CourseMajorType.Drums,
                new[]
                {
                    new CourseItem(
                        "История ударных",
                        new[]
                        {
                            new CourseTagItem("Бесплатно", CourseTagItemType.Paid),
                            new CourseTagItem("Видеокурс", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Первый урок",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
                            new CourseTagItem("Hard", CourseTagItemType.Hard),
                        }),
                }),
        };
    }

    public IMvxAsyncCommand<CourseItem> CourseTapCommand { get; }

    public MvxObservableCollection<BannerItem> BannersCollection { get; }

    public MvxObservableCollection<CourseGroupItem> CoursesGroupsCollection { get; }

    private Task CourseTapAsync(CourseItem item)
    {
        return NavigationService.Navigate<CourseDetailsViewModel>();
    }
}

public class CourseGroupItem : MvxNotifyPropertyChanged
{
    public CourseGroupItem(CourseMajorType majorType, IEnumerable<CourseItem> courses)
    {
        MajorType = majorType;
        Courses = courses;

        Major = majorType.GetDescription();
    }

    public CourseMajorType MajorType { get; }

    public string Major { get; }

    public IEnumerable<CourseItem> Courses { get; }
}

public class CourseItem : MvxNotifyPropertyChanged
{
    public CourseItem(string title, IEnumerable<CourseTagItem> tags)
    {
        Title = title;
        Tags = tags;
    }

    public string Title { get; }

    public IEnumerable<CourseTagItem> Tags { get; }
}

public class CourseTagItem
{
    public CourseTagItem(string title, CourseTagItemType type)
    {
        Type = type;
        Title = title;
    }

    public string Title { get; }

    public CourseTagItemType Type { get; }
}

public enum CourseMajorType
{
    [Description("Гитара")]
    Guitar,

    [Description("Валторна")]
    FrenchHorn,

    [Description("Ударные")]
    Drums,
}

public enum CourseTagItemType
{
    Free,

    Paid,

    Lite,

    Hard,

    Other,
}

public class BannerItem
{
}
