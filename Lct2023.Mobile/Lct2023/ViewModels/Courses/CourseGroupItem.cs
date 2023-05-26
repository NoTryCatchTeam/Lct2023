using System.Collections.Generic;
using Lct2023.Definitions.Types;
using Lct2023.Helpers;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

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
