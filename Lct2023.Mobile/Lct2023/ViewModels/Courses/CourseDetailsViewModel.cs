using System.Collections.Generic;
using DynamicData;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseDetailsViewModel : BaseViewModel<CourseDetailsViewModel.NavParameter>
{
    public CourseDetailsViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        CourseTagsCollection = new List<CourseTagItem>();
        CourseSectionsCollection = new MvxObservableCollection<CourseSectionItem>();
    }

    public IList<CourseTagItem> CourseTagsCollection { get; }

    public IList<CourseSectionItem> CourseSectionsCollection { get; }

    public override void Prepare(NavParameter parameter)
    {
        base.Prepare(parameter);

        CourseTagsCollection.AddRange(parameter.CourseItem.Tags);

        CourseSectionsCollection.AddRange(new CourseSectionItem[]
        {
            new ("Глава 1")
            {
                Lessons = new[]
                {
                    new CourseLessonItem("Знакомство с курсом")
                    {
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.Finished : CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Знакомство с устройством гитары: дека и гриф")
                    {
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.Finished : CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Техника постановки пальцев в баррэ")
                    {
                        Status = parameter.CourseItem.IsPurchased ? CourseLessonStatus.WaitingForReview : CourseLessonStatus.Locked,
                    },
                },
                IsPossibleToSync = parameter.CourseItem.IsPurchased,
            },
            new ("Глава 2")
            {
                Lessons = new[]
                {
                    new CourseLessonItem("Продолжение")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Гитара: дека и гриф")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Техника постановки пальцев в баррэ")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                },
            },
            new ("Глава 3")
            {
                Lessons = new[]
                {
                    new CourseLessonItem("Продолжение")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Гитара: дека и гриф")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                    new CourseLessonItem("Техника постановки пальцев в баррэ")
                    {
                        Status = CourseLessonStatus.Locked,
                    },
                },
            },
        });
    }

    public class NavParameter
    {
        public NavParameter(CourseItem courseItem)
        {
            CourseItem = courseItem;
        }

        public CourseItem CourseItem { get; }
    }
}

public class CourseSectionItem : MvxNotifyPropertyChanged
{
    public CourseSectionItem(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public bool IsPossibleToSync { get; set; }

    public IEnumerable<CourseLessonItem> Lessons { get; set; }
}

public class CourseLessonItem : MvxNotifyPropertyChanged
{
    private CourseLessonStatus _status;

    public CourseLessonItem(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public CourseLessonStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}

public enum CourseLessonStatus
{
    Available,

    Locked,

    Finished,

    WaitingForReview,
}
