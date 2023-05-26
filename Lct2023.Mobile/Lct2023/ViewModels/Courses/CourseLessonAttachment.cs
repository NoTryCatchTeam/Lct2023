using System;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonAttachment
{
    public string Name { get; set; }

    public string Extension { get; set; }

    public string Mime { get; set; }

    public string Url { get; set; }

    public bool IsVideo => Extension.Contains("mov", StringComparison.InvariantCultureIgnoreCase) ||
                           Extension.Contains("mp4", StringComparison.InvariantCultureIgnoreCase) ||
                           Mime.Contains("video", StringComparison.InvariantCultureIgnoreCase);
}