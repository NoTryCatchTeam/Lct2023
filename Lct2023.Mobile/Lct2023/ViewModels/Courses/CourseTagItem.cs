using Lct2023.Definitions.Types;

namespace Lct2023.ViewModels.Courses;

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