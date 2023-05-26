using System.Collections.Generic;
using DataModel.Definitions.Enums;
using DataModel.Responses.BaseCms;

namespace DataModel.Responses.Courses;

public class CourseItemResponse
{
    public string Name { get; set; }

    public int Price { get; set; }

    public CourseLevelType LevelType { get; set; }

    public bool IsOnline { get; set; }

    public string Description { get; set; }

    public CmsResponse<IEnumerable<CmsItemResponse<CourseLessonItem>>> Lessons { get; set; }
}
