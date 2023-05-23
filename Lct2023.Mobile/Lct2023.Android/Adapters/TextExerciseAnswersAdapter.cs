using System;
using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class TextExerciseAnswersAdapter : BaseRecyclerViewAdapter<BaseExerciseAnswer, TextExerciseAnswersAdapter.AnswerViewHolder>
{
    public TextExerciseAnswersAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, AnswerViewHolder> BindableViewHolderCreator =>
        (v, c) => new AnswerViewHolder(v, c);

    public class AnswerViewHolder : BaseViewHolder
    {
        public AnswerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var layout = itemView.FindViewById<MaterialCardView>(Resource.Id.text_exercise_answer_item_layout);
            var number = itemView.FindViewById<TextView>(Resource.Id.text_exercise_answer_item_number);
            var text = itemView.FindViewById<TextView>(Resource.Id.text_exercise_answer_item_text);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerViewHolder, TextExerciseAnswer>();

                set.Bind(layout)
                    .For(x => x.CardBackgroundColor)
                    .To(vm => vm.IsSelected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(isSelected =>
                        !isSelected ?
                            Application.Context.Resources.GetColorStateList(Resource.Color.backgroundDark) :
                            Application.Context.Resources.GetColorStateList(ViewModel.IsCorrect ? Resource.Color.accent : Resource.Color.alertDanger)));

                set.Bind(number)
                    .For(x => x.Text)
                    .To(vm => vm.Number);

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.Value);

                set.Apply();
            });
        }
    }
}
