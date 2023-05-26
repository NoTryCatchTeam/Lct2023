using System.Threading.Tasks;
using Lct2023.Definitions.Types;
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
                        })
                    {
                        IsPurchased = true,
                    },
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
                            new CourseTagItem("Бесплатно", CourseTagItemType.Free),
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
                            new CourseTagItem("Бесплатно", CourseTagItemType.Free),
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
        return NavigationService.Navigate<CourseDetailsViewModel, CourseDetailsViewModel.NavParameter>(
            new CourseDetailsViewModel.NavParameter(item));
    }
}
