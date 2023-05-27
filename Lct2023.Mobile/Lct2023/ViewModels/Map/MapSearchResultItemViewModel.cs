using DataModel.Definitions.Enums;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class MapSearchResultItemViewModel : MvxNotifyPropertyChanged
{
    public string Id { get; set; }
    
    public string Title { get; set;  }
    
    public string Address { get; set; }
    
    public string HexColor { get; set;  }
    
    public LocationType LocationType { get; set; }
}