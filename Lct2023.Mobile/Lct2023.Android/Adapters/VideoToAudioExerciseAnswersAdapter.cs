using System;
using System.Reactive.Disposables;
using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using DynamicData.Binding;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class VideoToAudioExerciseAnswersAdapter : BaseRecyclerViewAdapter<VideoToAudioExerciseAnswer, VideoToAudioExerciseAnswersAdapter.AnswerViewHolder>
{
    public VideoToAudioExerciseAnswersAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, AnswerViewHolder> BindableViewHolderCreator =>
        (v, c) => new AnswerViewHolder(v, c);

    public class AnswerViewHolder : BaseViewHolder
    {
        private readonly MaterialCardView _layout;
        private readonly LinearProgressIndicator _progress;
        private readonly TextView _number;

        private IDisposable _subscription;

        public AnswerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _layout = itemView.FindViewById<MaterialCardView>(Resource.Id.video_to_audio_exercise_answer_item_layout);
            _progress = itemView.FindViewById<LinearProgressIndicator>(Resource.Id.video_to_audio_exercise_answer_item_progress);
            _number = itemView.FindViewById<TextView>(Resource.Id.video_to_audio_exercise_answer_item_number);
            var icon = itemView.FindViewById<ImageView>(Resource.Id.video_to_audio_exercise_answer_item_icon);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerViewHolder, VideoToAudioExerciseAnswer>();

                set.Bind(_number)
                    .For(x => x.Text)
                    .To(vm => vm.Number);

                set.Bind(icon)
                    .For(x => x.ImageTintList)
                    .To(vm => vm.IsSelected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(isSelected =>
                        Application.Context.Resources.GetColorStateList(!isSelected ? Resource.Color.lightPurple : Resource.Color.background)));

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            _subscription ??= ViewModel.WhenChanged(
                    x => x.IsSelected,
                    x => x.IsPreSelected,
                    (_, isSelected, isPreSelected) => new { IsSelected = isSelected, IsPreSelected = isPreSelected })
                .Subscribe(x =>
                {
                    if (x.IsPreSelected)
                    {
                        // TODO Play audio
                        _layout.StrokeWidth = DimensUtils.DpToPx(Application.Context, 1);
                        _progress.Visibility = ViewStates.Visible;

                        return;
                    }

                    // TODO Stop audio
                    _layout.StrokeWidth = 0;
                    _progress.Visibility = ViewStates.Invisible;

                    if (x.IsSelected)
                    {
                        _layout.CardBackgroundColor = Application.Context.Resources.GetColorStateList(ViewModel.IsCorrect ? Resource.Color.accent : Resource.Color.alertDanger);
                    }
                })
                .DisposeWith(CompositeDisposable);
        }
    }
}
