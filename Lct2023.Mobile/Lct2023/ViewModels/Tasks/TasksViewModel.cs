using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Tasks;
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
    private readonly string _resourcesBaseUrl;

    private TasksViewState _state;
    private IEnumerable<CmsItemResponse<TaskOfTheDayItemResponse>> _taskOfTheDay;

    public TasksViewModel(
        ITasksRestService tasksRestService,
        IConfiguration configuration,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _tasksRestService = tasksRestService;
        var configuration1 = configuration;

        TaskOfTheDayCommand = new MvxAsyncCommand(TaskOfTheDayAsync);
        TasksFilterCommand = new MvxAsyncCommand(() => Task.CompletedTask);
        TaskTapCommand = new MvxAsyncCommand<TaskItem>(TaskTapAsync);

        TasksCollection = new MvxObservableCollection<TaskItem>();

        _resourcesBaseUrl =
            $"{configuration1.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{configuration1.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}"
                .TrimEnd('/');
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

        RunSafeTaskAsync(
            async () =>
            {
                var tasksResponse = await _tasksRestService.GetTasksAsync(CancellationToken);

                TasksCollection.AddRange(
                    tasksResponse
                        .Select((task, i) =>
                        {
                            return MapTaskResponse(task, i);
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
                _taskOfTheDay ??= await _tasksRestService.GetTaskOfTheDayAsync(CancellationToken);

                await TaskTapAsync(MapTaskResponse(_taskOfTheDay.First().Item.Task.Data, 0));
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

    private TaskItem MapTaskResponse(CmsItemResponse<TaskItemResponse> task, int i)
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
                            VideoUrl = $"{_resourcesBaseUrl}{quiz.Item.Video.Data.Item.Url}",
                            Answers = new[] { quiz.Item.AnswerA, quiz.Item.AnswerB, quiz.Item.AnswerC }
                                .Select((answer, j) => new VideoToAudioExerciseAnswer
                                {
                                    Number = j + 1,
                                    IsCorrect = correctAnswer == answer,
                                    AudioUrl = $"{_resourcesBaseUrl}{answer.Data.Item.Url}",
                                })
                                .ToList(),
                        };
                    }));

            tasksCounter += exercises.Count;
        }

        return new TaskItem(i + 1, exercises);
    }
}

[Flags]
public enum TasksViewState
{
    TaskOfTheDayLoading = 1 << 0,
}
