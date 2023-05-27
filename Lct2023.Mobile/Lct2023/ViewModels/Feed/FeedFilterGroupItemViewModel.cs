using System.Collections.ObjectModel;
using Lct2023.Definitions.Enums;
using Lct2023.ViewModels.Common;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Feed;

public class FeedFilterGroupItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }
    
    public FeedFilterGroupType FilterGroupType { get; set; }
    
    public ObservableCollection<FilterItemViewModel> Items { get; set; }
}