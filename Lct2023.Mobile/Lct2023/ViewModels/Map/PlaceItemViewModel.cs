using DataModel.Responses.Map;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class PlaceItemViewModel : MvxNotifyPropertyChanged 
{
    public PlaceItemViewModel(SchoolLocationResponse response)
    {
        Latitude = response.Latitude;
        Longitude = response.Longitude;
        Name = response.Name;
    }

    public double Latitude { get; }

    public double Longitude { get; }

    public string Name { get; }
}