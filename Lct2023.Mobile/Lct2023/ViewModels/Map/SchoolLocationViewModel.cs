using DataModel.Responses.Map;
using Lct2023.ViewModels.Base;
using MvvmCross.ViewModels;
using PropertyChanged;

namespace Lct2023.ViewModels.Map;

[DoNotNotify]
public class SchoolLocationItemViewModel : BaseItemViewModel 
{
    private readonly SchoolLocationResponse _item;

    public SchoolLocationItemViewModel(SchoolLocationResponse response) => _item = response;

    public double Latitude => _item.Latitude;

    public double Longitude => _item.Longitude;

    public string Name => _item.Name;
}