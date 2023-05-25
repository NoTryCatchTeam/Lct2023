using DataModel.Definitions.Enums;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class SocialLinkItemViewModel : MvxNotifyPropertyChanged
{
    public SocialLinkTypes SocialLinkType { get; set; }
    
    public string Url { get; set; }
    
    public IMvxCommand OpenSiteCommand { get; set; }
}