using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonItem : MvxNotifyPropertyChanged
{
    private CourseLessonStatus _status;
    private CourseLessonResolution _resolution;

    public CourseLessonItem()
    {
        ConversationItems = new MvxObservableCollection<CourseLessonConversation>();
    }

    public string Title { get; set; }

    public string Description { get; set; }

    public int Number { get; set; }

    public int SectionNumber { get; set; }

    public string AdditionalDescription { get; set; }

    public string HomeworkDescription { get; set; }

    public CourseLessonAttachment Attachment { get; set; }

    public CourseLessonResolution Resolution
    {
        get => _resolution;
        set => SetProperty(ref _resolution, value);
    }

    public MvxObservableCollection<CourseLessonConversation> ConversationItems { get; }

    public CourseLessonStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}
