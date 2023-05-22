using System;
using Android.Views;
using Android.Widget;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class TasksAdapter : BaseRecyclerViewAdapter<TaskItem, TasksAdapter.TaskViewHolder>
{
    public TasksAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, TaskViewHolder> BindableViewHolderCreator =>
        (v, c) => new TaskViewHolder(v, c);

    public class TaskViewHolder : BaseViewHolder
    {
        public TaskViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var progress = itemView.FindViewById<TextView>(Resource.Id.task_list_item_progress);
            var number = itemView.FindViewById<TextView>(Resource.Id.task_list_item_number);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(progress)
                    .For(x => x.Text)
                    .To(vm => vm)
                    .WithConversion(new AnyExpressionConverter<TaskItem, string>(x => $"{x.CompletedExercises}/{x.TotalExercises}"));

                set.Bind(number)
                    .For(x => x.Text)
                    .To(vm => vm.Number);

                set.Apply();
            });
        }
    }
}
