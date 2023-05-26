using System;
using System.Linq;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public class TasksViewModel : BaseViewModel
{
    private readonly ITasksRestService _tasksRestService;

    private TasksViewState _state;

    public TasksViewModel(
        ITasksRestService tasksRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _tasksRestService = tasksRestService;
        TaskOfTheDayCommand = new MvxAsyncCommand(TaskOfTheDayAsync);

        TasksFilterCommand = new MvxAsyncCommand(() => Task.CompletedTask);

        TaskTapCommand = new MvxAsyncCommand<TaskItem>(item => Task.CompletedTask);

        TasksCollection = new MvxObservableCollection<TaskItem>();

        TasksCollection.AddRange(new (int completed, int total)[]
        {
            (1, 10),
            (2, 10),
            (3, 10),
            (1, 10),
            (5, 10),
            (10, 10),
            (1, 10),
            (9, 10),
            (10, 10),
            (0, 10),
        }.Select((x, i) => new TaskItem(i + 1) { CompletedExercises = x.completed, TotalExercises = x.total }));
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

    private async Task TaskOfTheDayAsync()
    {
        State |= TasksViewState.TaskOfTheDayLoading;

        await RunSafeTaskAsync(
            async () =>
            {
                var tasksResponse = await _tasksRestService.GetTasksAsync(CancellationToken);

                await NavigationService.Navigate<TaskDetailsViewModel, TaskDetailsViewModel.NavParameter>(
                    new TaskDetailsViewModel.NavParameter(tasksResponse.First(x => x.Item.VideoQuizzes.Data.Any())));
            });

        State &= ~TasksViewState.TaskOfTheDayLoading;
    }
}

[Flags]
public enum TasksViewState
{
    TaskOfTheDayLoading = 1 << 0,
}
