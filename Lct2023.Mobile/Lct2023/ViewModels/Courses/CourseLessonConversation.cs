using System;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonConversation
{
    public CourseLessonConversation()
    {
        CreatedAt = DateTimeOffset.Now;
    }

    public ConversationAuthor Author { get; set; }

    public DateTimeOffset CreatedAt { get; }

    public string Text { get; set; }
}