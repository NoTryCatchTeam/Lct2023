using System;
using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class AudioToPictureExerciseAnswersAdapter : BaseRecyclerViewAdapter<AudioToPictureExerciseAnswer, AudioToPictureExerciseAnswersAdapter.AnswerViewHolder>
{
    public AudioToPictureExerciseAnswersAdapter(IMvxAndroidBindingContext bindingContext)
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
            var layout = itemView.FindViewById<MaterialCardView>(Resource.Id.audio_to_picture_exercise_answer_item_layout);
            var text = itemView.FindViewById<TextView>(Resource.Id.audio_to_picture_exercise_answer_item_text);
            var image = itemView.FindViewById<ImageView>(Resource.Id.audio_to_picture_exercise_answer_item_image);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerViewHolder, AudioToPictureExerciseAnswer>();

                set.Bind(layout)
                    .For(x => x.CardBackgroundColor)
                    .To(vm => vm.IsSelected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(isSelected =>
                        !isSelected ?
                            Application.Context.Resources.GetColorStateList(Resource.Color.backgroundDark) :
                            Application.Context.Resources.GetColorStateList(ViewModel.IsCorrect ? Resource.Color.accent : Resource.Color.alertDanger)));

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.PictureDescription);

                set.Bind(image)
                    .For(nameof(ImageByUrlTargetBinding))
                    .To(vm => vm.PictureUrl);

                set.Apply();
            });
        }
    }
}
