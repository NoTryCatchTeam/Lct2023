using System.Collections.ObjectModel;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map.Filters;

public class MapFilterSubGroupItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }
    
    public bool IsSelected { get; set; }
    
    public bool IsOpened { get; set; }
    
    public ObservableCollection<MapFilterItemViewModel> Items { get; set; }
}