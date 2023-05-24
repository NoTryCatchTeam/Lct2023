using System;
using System.Reactive.Disposables;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DynamicData.Binding;
using Google.Android.Material.Card;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class TaskDetailsCounterAdapter : BaseRecyclerViewAdapter<BaseExerciseItem, TaskDetailsCounterAdapter.ExerciseViewHolder>
{
    public TaskDetailsCounterAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, ExerciseViewHolder> BindableViewHolderCreator =>
        (v, c) => new ExerciseViewHolder(v, c);

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var holder = base.OnCreateViewHolder(parent, viewType);

        if (holder.ItemView is { } view && ItemCount >= 10)
        {
            var itemLayoutParams = (ViewGroup.MarginLayoutParams)view.LayoutParameters;
            itemLayoutParams.Width = (parent.MeasuredWidth -
                                      parent.PaddingLeft -
                                      parent.PaddingRight -
                                      DimensUtils.DpToPx(parent.Context, 6) * 9) / 10;
        }

        return holder;
    }

    public class ExerciseViewHolder : BaseViewHolder
    {
        private readonly MaterialCardView _layout;
        private readonly TextView _text;

        private IDisposable _subscription;

        public ExerciseViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _layout = itemView.FindViewById<MaterialCardView>(Resource.Id.task_details_progress_item_layout);
            _text = itemView.FindViewById<TextView>(Resource.Id.task_details_progress_item_number);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(_text)
                    .For(x => x.Text)
                    .To(vm => vm.Number);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            _subscription ??= ViewModel.WhenChanged(
                    x => x.IsCorrect,
                    x => x.IsCurrent,
                    (_, isCorrect, isCurrent) => new { IsCorrect = isCorrect, IsCurrent = isCurrent })
                .Subscribe(x =>
                {
                    if (x.IsCorrect == null && !x.IsCurrent)
                    {
                        return;
                    }

                    if (x.IsCorrect == null)
                    {
                        _layout.CardBackgroundColor = Application.Context.Resources.GetColorStateList(Resource.Color.background, null);
                        _layout.SetStrokeColor(Application.Context.Resources.GetColorStateList(Resource.Color.accent, null));
                        _layout.StrokeWidth = DimensUtils.DpToPx(Application.Context, 1);

                        return;
                    }

                    _layout.CardBackgroundColor = Application.Context.Resources.GetColorStateList(x.IsCorrect.Value ? Resource.Color.accent : Resource.Color.alertDanger, null);
                    _layout.StrokeWidth = 0;

                    _text.SetTextColor(Application.Context.Resources.GetColorStateList(Resource.Color.textLight, null));
                })
                .DisposeWith(CompositeDisposable);
        }
    }
}
