using System.Collections.ObjectModel;
using Lct2023.Definitions.Enums;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map.Filters;

public class MapFilterGroupItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }
    
    public bool IsOpened { get; set; }
    
    public MapFilterGroupType FilterGroupType { get; set; }
    
    public ObservableCollection<MapFilterSubGroupItemViewModel> SubGroups { get; set; }
}