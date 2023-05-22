using System.Linq;
using System.Threading.Tasks;
using Lct2023.ViewModels.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tests;

public class TasksViewModel : BaseViewModel
{
    public TasksViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        TaskOfTheDayCommand = new MvxAsyncCommand(() => Task.CompletedTask);

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
}
