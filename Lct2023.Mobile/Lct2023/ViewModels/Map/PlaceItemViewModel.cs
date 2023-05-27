using System.Collections.Generic;
using DataModel.Definitions.Enums;
using DataModel.Responses.Map;
using Lct2023.ViewModels.Art;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class PlaceItemViewModel : MvxNotifyPropertyChanged 
{
    public string HexColor { get; set;  }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Title { get; set;  }
    
    public string Description { get; set; }
    
    public string Address { get; set; }
    
    public IEnumerable<string> Contacts { get; set; }
    
    public string Email { get; set; }
    
    public string Site { get; set; }
    
    public IEnumerable<SocialLinkResponse> SocialLinks { get; set; }
    
    public IEnumerable<StreamItemViewModel> Streams { get; set; }
    
    public LocationType LocationType { get; set; }
    
    public string Id { get; set; }

    public string TicketLink { get; set; }
}