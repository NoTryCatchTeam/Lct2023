using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map.Filters;

public class MapFilterItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }
    
    public bool IsSelected { get; set; }
}