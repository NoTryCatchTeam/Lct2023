using System.Collections.Generic;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonSectionItem : MvxNotifyPropertyChanged
{
    public CourseLessonSectionItem(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public bool IsPossibleToSync { get; set; }

    public IEnumerable<CourseLessonItem> Lessons { get; set; }
}