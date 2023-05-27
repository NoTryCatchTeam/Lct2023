using System.Collections.Generic;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseItem : MvxNotifyPropertyChanged
{
    private bool _isPurchased;

    public CourseItem(string title, IEnumerable<CourseTagItem> tags)
    {
        Title = title;
        Tags = tags;
    }

    public string Title { get; }

    public IEnumerable<CourseTagItem> Tags { get; }

    public IEnumerable<CourseLessonItem> Lessons { get; set; }

    public bool IsPurchased
    {
        get => _isPurchased;
        set => SetProperty(ref _isPurchased, value);
    }
}
