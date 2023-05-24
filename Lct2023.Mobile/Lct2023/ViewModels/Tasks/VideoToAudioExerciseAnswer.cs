namespace Lct2023.ViewModels.Tasks;

public class VideoToAudioExerciseAnswer : BaseExerciseAnswer
{
    private bool _isPreSelected;

    public string AudioUrl { get; set; }

    public bool IsPreSelected
    {
        get => _isPreSelected;
        set => SetProperty(ref _isPreSelected, value);
    }
}