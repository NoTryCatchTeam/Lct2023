namespace Lct2023.ViewModels.Tasks;

public class TaskItem
{
    public TaskItem(int number)
    {
        Number = number;
    }

    public int Number { get; }

    public int TotalExercises { get; set; }

    public int CompletedExercises { get; set; }

    public bool IsCompleted => CompletedExercises == TotalExercises;
}
