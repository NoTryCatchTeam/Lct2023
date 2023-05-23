using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public abstract class BaseExerciseAnswer : MvxNotifyPropertyChanged
{
    private bool _isSelected;

    public int Number { get; set; }

    public bool IsCorrect { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}