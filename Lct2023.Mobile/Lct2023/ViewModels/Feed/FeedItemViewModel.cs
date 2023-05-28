using System.Collections.Generic;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Feed;

public class FeedItemViewModel : MvxNotifyPropertyChanged
{
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string PublishedAt { get; set; }
    
    public string Link { get; set; }
    
    public string ImageUrl { get; set; }
    
    public IEnumerable<string> ArtCategories { get; set; }
    
    public IMvxCommand ClickCommand { get; set; }
}