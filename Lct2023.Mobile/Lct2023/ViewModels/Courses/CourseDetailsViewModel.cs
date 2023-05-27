using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseDetailsViewModel : BaseViewModel<CourseDetailsViewModel.NavParameter>
{
    public CourseDetailsViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        LessonTapCommand = new MvxAsyncCommand<CourseLessonItem>(LessonTapAsync);

        CourseTagsCollection = new List<CourseTagItem>();
        CourseSectionsCollection = new MvxObservableCollection<CourseLessonSectionItem>();
    }

    public IMvxAsyncCommand<CourseLessonItem> LessonTapCommand { get; }

    public IList<CourseTagItem> CourseTagsCollection { get; }

    public IList<CourseLessonSectionItem> CourseSectionsCollection { get; }

    public override void Prepare(NavParameter parameter)
    {
        base.Prepare(parameter);

        CourseTagsCollection.AddRange(parameter.CourseItem.Tags);

        var sections = new List<CourseLessonSectionItem>();

        if (parameter.CourseItem.Lessons is { } lessons && lessons.Any())
        {
            var courseLessonItems = lessons.ToArray();

            sections.AddRange(courseLessonItems.GroupBy(x => x.SectionNumber)
                .Select(lessonGroup => new CourseLessonSectionItem($"Глава {lessonGroup.Key}")
                {
                    Lessons = courseLessonItems,
                }));
        }
        else
        {
            sections.AddRange(MockSectionItems(parameter));
        }

        CourseSectionsCollection.AddRange(sections);
    }

    private Task LessonTapAsync(CourseLessonItem item) =>
        NavigationService.Navigate<CourseLessonViewModel, CourseLessonViewModel.NavParameter>(new CourseLessonViewModel.NavParameter(item));

    private IEnumerable<CourseLessonSectionItem> MockSectionItems(NavParameter parameter) =>
        new CourseLessonSectionItem[]
        {
            new ("Глава 1")
            {
                Lessons = new[]
                {
                    new CourseLessonItem
                    {
                        Title = "Знакомство с курсом",
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.Finished : CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Знакомство с устройством гитары: дека и гриф",
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.Finished : CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Техника постановки пальцев в баррэ",
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.WaitingForReview : CourseLessonStatus.Locked,
                    },
                },
                IsPossibleToSync = parameter.CourseItem.IsPurchased,
            },
            new ("Глава 2")
            {
                Lessons = new[]
                {
                    new CourseLessonItem
                    {
                        Title = "Продолжение",
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Гитара: дека и гриф",
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Техника постановки пальцев в баррэ",
                        Status = CourseLessonStatus.Locked,
                    },
                },
            },
            new ("Глава 3")
            {
                Lessons = new[]
                {
                    new CourseLessonItem
                    {
                        Title = "Продолжение",
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Гитара: дека и гриф",
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem
                    {
                        Title = "Техника постановки пальцев в баррэ",
                        Status = CourseLessonStatus.Locked,
                    },
                },
            },
        };

    public class NavParameter
    {
        public NavParameter(CourseItem courseItem)
        {
            CourseItem = courseItem;
        }

        public CourseItem CourseItem { get; }
    }
}
