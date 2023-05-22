using DataModel.Responses.Stories;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Main;

public class StoryQuizItemViewModel : MvxNotifyPropertyChanged, IStoryCardItemViewModel
{
    public StoryQuizItemViewModel(StoryQuizResponse item)
    {
        Item = item;
    }

    public string Title => Item.Title;

    public StoryQuizResponse Item { get;}
}