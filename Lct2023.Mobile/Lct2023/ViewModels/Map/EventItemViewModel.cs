using System;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class EventItemViewModel : MvxNotifyPropertyChanged
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public string ImageUrl { get; set; }

    public DateTime EventDate { get; set; }

    public IMvxCommand OpenSiteCommand { get; set; }
}
