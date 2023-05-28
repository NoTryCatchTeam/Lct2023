using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Common;

public class FilterItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }
    
    public bool IsSelected { get; set; }
}