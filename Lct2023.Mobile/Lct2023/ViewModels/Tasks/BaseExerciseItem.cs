using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public abstract class BaseExerciseItem : MvxNotifyPropertyChanged
{
    private bool? _isCorrect;

    public IMvxAsyncCommand<BaseExerciseAnswer> AnswerTapCommand { get; set; }

    public int Number { get; set; }

    public bool? IsCorrect
    {
        get => _isCorrect;
        set => SetProperty(ref _isCorrect, value);
    }

    public bool IsCurrent { get; set; }

    public string Question { get; set; }

    public string DescriptionOfCorrectness { get; set; }
}
