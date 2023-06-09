using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Tasks;
using Lct2023.Definitions.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public class TasksViewModel : BaseMainTabViewModel
{
    private readonly ITasksRestService _tasksRestService;
    private readonly IConfiguration _configuration;

    private TasksViewState _state;

    public TasksViewModel(
        ITasksRestService tasksRestService,
        IConfiguration configuration,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _tasksRestService = tasksRestService;
        _configuration = configuration;

        TaskOfTheDayCommand = new MvxAsyncCommand(TaskOfTheDayAsync);
        TasksFilterCommand = new MvxAsyncCommand(() => Task.CompletedTask);
        TaskTapCommand = new MvxAsyncCommand<TaskItem>(TaskTapAsync);

        TasksCollection = new MvxObservableCollection<TaskItem>();
    }

    public IMvxAsyncCommand TaskOfTheDayCommand { get; }

    public IMvxAsyncCommand TasksFilterCommand { get; }

    public IMvxAsyncCommand<TaskItem> TaskTapCommand { get; }

    public MvxObservableCollection<TaskItem> TasksCollection { get; }

    public TasksViewState State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    public override void ViewCreated()
    {
        base.ViewCreated();

        var resourcesBaseUrl =
            $"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}"
                .TrimEnd('/');

        RunSafeTaskAsync(
            async () =>
            {
                var tasksResponse = await _tasksRestService.GetTasksAsync(CancellationToken);

                TasksCollection.AddRange(
                    tasksResponse
                        .Select((task, i) =>
                        {
                            var exercises = new List<BaseExerciseItem>();

                            var tasksCounter = 0;

                            // Text quizzes
                            if (task.Item.Quizzes?.Data?.Any() == true)
                            {
                                exercises.AddRange(
                                    task.Item.Quizzes.Data
                                        .Select((quiz, i) => new TextExerciseItem
                                        {
                                            Number = i + 1,
                                            Question = quiz.Item.Question,
                                            DescriptionOfCorrectness = quiz.Item.Explanation,
                                            Answers = new[] { quiz.Item.AnswerA, quiz.Item.AnswerB, quiz.Item.AnswerC, quiz.Item.AnswerD }
                                                .Select((answer, j) => new TextExerciseAnswer
                                                {
                                                    Number = j + 1,
                                                    IsCorrect = quiz.Item.CorrectAnswerValue == answer,
                                                    Value = answer,
                                                })
                                                .ToList(),
                                        }));

                                tasksCounter += exercises.Count;
                            }

                            // Video to audio quizzes
                            if (task.Item.Quizzes?.Data?.Any() == true)
                            {
                                exercises.AddRange(
                                    task.Item.VideoQuizzes.Data
                                        .Select((quiz, i) =>
                                        {
                                            var correctAnswerTag = quiz.Item.CorrectAnswerTag;

                                            var correctAnswer = correctAnswerTag switch
                                            {
                                                "a" => quiz.Item.AnswerA,
                                                "b" => quiz.Item.AnswerB,
                                                _ => quiz.Item.AnswerC,
                                            };

                                            return new VideoToAudioExerciseItem
                                            {
                                                Number = tasksCounter + i + 1,
                                                Question = quiz.Item.Question,
                                                VideoUrl = $"{resourcesBaseUrl}{quiz.Item.Video.Data.Item.Url}",
                                                Answers = new[] { quiz.Item.AnswerA, quiz.Item.AnswerB, quiz.Item.AnswerC }
                                                    .Select((answer, j) => new VideoToAudioExerciseAnswer
                                                    {
                                                        Number = j + 1,
                                                        IsCorrect = correctAnswer == answer,
                                                        AudioUrl = $"{resourcesBaseUrl}{answer.Data.Item.Url}",
                                                    })
                                                    .ToList(),
                                            };
                                        }));

                                tasksCounter += exercises.Count;
                            }

                            return new TaskItem(i + 1, exercises);
                        })
                        .ToList());

                await RaisePropertyChanged(nameof(TasksCollection));
            });
    }

    private async Task TaskOfTheDayAsync()
    {
        State |= TasksViewState.TaskOfTheDayLoading;

        await RunSafeTaskAsync(
            async () =>
            {
                // TODO Add task of the day loading
                // var tasksResponse = await _tasksRestService.GetTasksAsync(CancellationToken);
                //
                // await NavigationService.Navigate<TaskDetailsViewModel, TaskDetailsViewModel.NavParameter>(
                //     new TaskDetailsViewModel.NavParameter(TasksCollection.First()));
            });

        State &= ~TasksViewState.TaskOfTheDayLoading;
    }

    private async Task TaskTapAsync(TaskItem item)
    {
        if (item.IsCompleted)
        {
            return;
        }

        await NavigationService.Navigate<TaskDetailsViewModel, TaskDetailsViewModel.NavParameter, TaskDetailsViewModel.NavBackParameter>(
            new TaskDetailsViewModel.NavParameter(item));

        await RaisePropertyChanged(nameof(TasksCollection));
    }
}

[Flags]
public enum TasksViewState
{
    TaskOfTheDayLoading = 1 << 0,
}
