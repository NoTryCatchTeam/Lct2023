using Lct2023.Definitions.Enums;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class SocialLinkItemViewModel : MvxNotifyPropertyChanged
{
    public SocialLinkTypes SocialLinkType { get; set; }
    
    public string Url { get; set; }
}