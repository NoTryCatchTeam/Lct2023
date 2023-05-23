using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public class TaskDetailsViewModel : BaseViewModel
{
    private BaseExerciseItem _currentExercise;

    public TaskDetailsViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        var answerTapCommand = new MvxAsyncCommand<BaseExerciseAnswer>(AnswerTapAsync);

        CallToActionCommand = new MvxAsyncCommand(CallToActionAsync);

        var exercises = new List<BaseExerciseItem>(10);

        for (var i = 0; i < exercises.Capacity; i++)
        {
            BaseExerciseItem exercise;

            // Audio to picture exercise
            if (i % 4 == 0)
            {
                var answers = new List<AudioToPictureExerciseAnswer>();

                for (var j = 0; j < 4; j++)
                {
                    answers.Add(new AudioToPictureExerciseAnswer
                    {
                        Number = j + 1,
                        IsCorrect = j == 2,
                        PictureUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRe7les3XfVDfSQnIrZ6r5N_ayWlVD_iJ5teQ&usqp=CAU",
                        PictureDescription = $"Может Бах #{j}",
                    });
                }

                exercise = new AudioToPictureExerciseItem
                {
                    Number = i + 1,
                    Question = "Кто исполняет?",
                    DescriptionOfCorrectness = "Ха-ха-ха",
                    Answers = answers,
                };
            }
            // Video to audio exercise
            else if (i % 3 == 0)
            {
                var answers = new List<VideoToAudioExerciseAnswer>();

                for (var j = 0; j < 4; j++)
                {
                    answers.Add(new VideoToAudioExerciseAnswer
                    {
                        Number = j + 1,
                        IsCorrect = j == 2,
                        AudioUrl = "https://cdn.pixabay.com/download/audio/2023/05/18/audio_473dc360c2.mp3",
                    });
                }

                exercise = new VideoToAudioExerciseItem
                {
                    Number = i + 1,
                    Question = "На видео будет Бах, а ты пока тыкай наугад",
                    DescriptionOfCorrectness = "Ха-ха-ха",
                    Answers = answers,
                };
            }
            // Text exercise
            else
            {
                var answers = new List<TextExerciseAnswer>();

                for (var j = 0; j < 4; j++)
                {
                    answers.Add(new TextExerciseAnswer
                    {
                        Number = j + 1,
                        IsCorrect = j == 2,
                        Value = j == 0 ?
                            "Мадонна" :
                            j == 1 ?
                                "Бетховен" :
                                j == 2 ?
                                    "Самоубийство" :
                                    "Арни",
                    });
                }

                exercise = new TextExerciseItem
                {
                    Number = i + 1,
                    Question = "Кто убил голого Баха?",
                    DescriptionOfCorrectness = "Ха-ха-ха",
                    Answers = answers,
                };
            }

            exercise.AnswerTapCommand = answerTapCommand;

            exercises.Add(exercise);
        }

        ExercisesCollection = exercises;
    }

    private Task AnswerTapAsync(BaseExerciseAnswer item)
    {
        if (_currentExercise.IsCorrect != null)
        {
            return Task.CompletedTask;
        }

        switch (item)
        {
            case TextExerciseAnswer answer:
                answer.IsSelected = true;
                _currentExercise.IsCorrect = answer.IsCorrect;

                if (!answer.IsCorrect)
                {
                    ((TextExerciseItem)_currentExercise).Answers.First(x => x.IsCorrect).IsSelected = true;
                }

                break;
            case AudioToPictureExerciseAnswer answer:
                answer.IsSelected = true;
                _currentExercise.IsCorrect = answer.IsCorrect;

                if (!answer.IsCorrect)
                {
                    ((AudioToPictureExerciseItem)_currentExercise).Answers.First(x => x.IsCorrect).IsSelected = true;
                }

                break;
            case VideoToAudioExerciseAnswer answer:
                var exercise = (VideoToAudioExerciseItem)_currentExercise;

                if (exercise.Answers.FirstOrDefault(x => x.IsPreSelected) is { } preSelectedAnswer)
                {
                    preSelectedAnswer.IsPreSelected = false;
                }

                answer.IsPreSelected = true;

                RaisePropertyChanged(nameof(IsPreSelectedAnswer));

                break;
        }

        return Task.CompletedTask;
    }

    private async Task CallToActionAsync()
    {
        // Next exercise or finish task
        if (_currentExercise.IsCorrect != null)
        {
            if (ExercisesCollection.FirstOrDefault(x => x.Number == _currentExercise.Number + 1) is { } nextExercise)
            {
                CurrentExercise = nextExercise;
            }
            else
            {
                await NavigationService.Close(this);
            }

            return;
        }

        // Answer preselected, select it
        if (_currentExercise is VideoToAudioExerciseItem videoToAudioExerciseItem && videoToAudioExerciseItem.Answers.FirstOrDefault(x => x.IsPreSelected) is { } preSelectedAnswer)
        {
            preSelectedAnswer.IsPreSelected = false;
            await RaisePropertyChanged(nameof(IsPreSelectedAnswer));

            preSelectedAnswer.IsSelected = true;

            _currentExercise.IsCorrect = preSelectedAnswer.IsCorrect;

            if (!preSelectedAnswer.IsCorrect)
            {
                videoToAudioExerciseItem.Answers.First(x => x.IsCorrect).IsSelected = true;
            }
        }
    }

    public IMvxAsyncCommand CallToActionCommand { get; }

    public IEnumerable<BaseExerciseItem> ExercisesCollection { get; }

    public BaseExerciseItem CurrentExercise
    {
        get => _currentExercise;
        private set
        {
            if (value != null)
            {
                value.IsCurrent = false;
            }

            SetProperty(ref _currentExercise, value);

            if (_currentExercise != null)
            {
                _currentExercise.IsCurrent = true;
            }
        }
    }

    public bool IsPreSelectedAnswer => (CurrentExercise as VideoToAudioExerciseItem)?.Answers.Any(x => x.IsPreSelected) == true;

    public override void ViewCreated()
    {
        base.ViewCreated();

        // TODO Set after loading completed
        CurrentExercise = ExercisesCollection.First();
    }
}

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

public abstract class ExerciseItem<TExerciseAnswer> : BaseExerciseItem
{
    public IEnumerable<TExerciseAnswer> Answers { get; set; }
}

public class TextExerciseItem : ExerciseItem<TextExerciseAnswer>
{
}

public class VideoToAudioExerciseItem : ExerciseItem<VideoToAudioExerciseAnswer>
{
    public string VideoUrl { get; set; }
}

public class AudioToPictureExerciseItem : ExerciseItem<AudioToPictureExerciseAnswer>
{
    public string AudioUrl { get; set; }
}

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

public class TextExerciseAnswer : BaseExerciseAnswer
{
    public string Value { get; set; }
}

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

public class AudioToPictureExerciseAnswer : BaseExerciseAnswer
{
    public string PictureUrl { get; set; }

    public string PictureDescription { get; set; }
}
