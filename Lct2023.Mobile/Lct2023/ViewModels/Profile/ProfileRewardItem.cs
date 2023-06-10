using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Profile;

public class ProfileRewardItem : MvxNotifyPropertyChanged
{
    private bool _isCollected;
    private int _points;
    private bool _isAvailable;

    public bool IsCollected
    {
        get => _isCollected;
        set => SetProperty(ref _isCollected, value);
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        set => SetProperty(ref _isAvailable, value);
    }

    public int Points
    {
        get => _points;
        set => SetProperty(ref _points, value);
    }
}
