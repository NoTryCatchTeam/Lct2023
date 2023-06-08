using System.Collections.Generic;
using System.Linq;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Tasks;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Tasks;

public class TaskItem : MvxNotifyPropertyChanged
{
    private int _completedExercises;

    public TaskItem(int number, IEnumerable<BaseExerciseItem> exercises)
    {
        Number = number;
        Exercises = exercises.ToList();

        TotalExercises = Exercises.Count();
    }

    public int Number { get; }

    public IEnumerable<BaseExerciseItem> Exercises { get; }

    public int TotalExercises { get; }

    public int CompletedExercises
    {
        get => _completedExercises;
        set
        {
            if (SetProperty(ref _completedExercises, value))
            {
                RaisePropertyChanged(nameof(IsCompleted));
            }
        }
    }

    public bool IsCompleted => CompletedExercises == TotalExercises;
}
