using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Quizzes;
using DataModel.Responses.Tasks;
using Lct2023.Definitions.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Tasks;

public class TaskDetailsViewModel : BaseViewModel<TaskDetailsViewModel.NavParameter>
{
    private readonly IConfiguration _configuration;

    private BaseExerciseItem _currentExercise;

    public TaskDetailsViewModel(
        IConfiguration configuration,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _configuration = configuration;
        CallToActionCommand = new MvxAsyncCommand(CallToActionAsync);
    }

    public IMvxAsyncCommand CallToActionCommand { get; }

    public IEnumerable<BaseExerciseItem> ExercisesCollection { get; private set; }

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

    public override void Prepare(NavParameter parameter)
    {
        base.Prepare(parameter);

        var answerTapCommand = new MvxAsyncCommand<BaseExerciseAnswer>(AnswerTapAsync);

        foreach (var taskExercise in parameter.Task.Exercises)
        {
            taskExercise.AnswerTapCommand = answerTapCommand;
        }

        ExercisesCollection = parameter.Task.Exercises;
    }

    public override void ViewCreated()
    {
        base.ViewCreated();

        CurrentExercise = ExercisesCollection.First(x => x.IsCorrect == null);
    }

    private Task AnswerTapAsync(BaseExerciseAnswer item)
    {
        if (_currentExercise.IsCorrect != null)
        {
            return Task.CompletedTask;
        }

        switch (item)
        {
            case TextExerciseAnswer { IsSelected: false } answer:
                answer.IsSelected = true;
                _currentExercise.IsCorrect = answer.IsCorrect;

                if (!answer.IsCorrect)
                {
                    ((TextExerciseItem)_currentExercise).Answers.First(x => x.IsCorrect).IsSelected = true;
                }

                break;
            case AudioToPictureExerciseAnswer { IsSelected: false } answer:
                answer.IsSelected = true;
                _currentExercise.IsCorrect = answer.IsCorrect;

                if (!answer.IsCorrect)
                {
                    ((AudioToPictureExerciseItem)_currentExercise).Answers.First(x => x.IsCorrect).IsSelected = true;
                }

                break;
            case VideoToAudioExerciseAnswer { IsPreSelected: false } answer:
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
            if (ExercisesCollection.FirstOrDefault(x => x.Number >= _currentExercise.Number + 1 && x.IsCorrect == null) is { } nextExercise)
            {
                CurrentExercise = nextExercise;
            }
            else
            {
                NavigationParameter.Task.CompletedExercises = ExercisesCollection.Count(x => x.IsCorrect == true);

                await NavigationService.Close(this);

                ClearTaskProgress();
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

    private void ClearTaskProgress()
    {
        foreach (var exerciseItem in NavigationParameter.Task.Exercises.Where(x => x.IsCorrect == false))
        {
            exerciseItem.IsCorrect = null;

            switch (exerciseItem)
            {
                case AudioToPictureExerciseItem audioToPictureExerciseItem:
                    foreach (var exerciseAnswer in audioToPictureExerciseItem.Answers)
                    {
                        exerciseAnswer.IsSelected = false;
                    }

                    break;
                case TextExerciseItem textExerciseItem:
                    foreach (var exerciseAnswer in textExerciseItem.Answers)
                    {
                        exerciseAnswer.IsSelected = false;
                    }

                    break;
                case VideoToAudioExerciseItem videoToAudioExerciseItem:
                    foreach (var exerciseAnswer in videoToAudioExerciseItem.Answers)
                    {
                        exerciseAnswer.IsSelected = false;
                        exerciseAnswer.IsPreSelected = false;
                    }

                    break;
            }
        }
    }

    public class NavParameter
    {
        public NavParameter(TaskItem task)
        {
            Task = task;
        }

        public TaskItem Task { get; }
    }
}
