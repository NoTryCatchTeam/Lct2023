using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseDetailsViewModel : BaseViewModel
{
    public CourseDetailsViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        CourseTagsCollection = new[]
        {
            new CourseTagItem("Платно", CourseTagItemType.Paid),
            new CourseTagItem("Презентация", CourseTagItemType.Other),
            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
            new CourseTagItem("Hard", CourseTagItemType.Hard),
        };

        CourseSectionsCollection = new MvxObservableCollection<CourseSectionItem>
        {
            new ("Глава 1")
            {
                Lessons = new[]
                {
                    new CourseLessonItem("Знакомство с курсом")
                    {
                        Status = CourseLessonStatus.WaitingForReview,
                    },
                    new CourseLessonItem("Знакомство с устройством гитары: дека и гриф")
                    {
                        Status = CourseLessonStatus.Finished,
                    },
                    new CourseLessonItem("Техника постановки пальцев в баррэ")
                    {
                        Status = CourseLessonStatus.Available,
                    },
                },
                IsPossibleToSync = true,
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
        };
    }

    public IEnumerable<CourseTagItem> CourseTagsCollection { get; }

    public MvxObservableCollection<CourseSectionItem> CourseSectionsCollection { get; }
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
