using System.Collections.Generic;

namespace Lct2023.ViewModels.Tasks;

public abstract class ExerciseItem<TExerciseAnswer> : BaseExerciseItem
{
    public IEnumerable<TExerciseAnswer> Answers { get; set; }
}
