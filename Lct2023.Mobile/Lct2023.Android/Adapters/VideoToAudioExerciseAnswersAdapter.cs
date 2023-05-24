using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.Media;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DynamicData.Binding;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Adapters;

public class VideoToAudioExerciseAnswersAdapter : BaseRecyclerViewAdapter<VideoToAudioExerciseAnswer, VideoToAudioExerciseAnswersAdapter.AnswerViewHolder>
{
    private readonly Func<MediaPlayer> _resetPlayer;

    public VideoToAudioExerciseAnswersAdapter(IMvxAndroidBindingContext bindingContext, Func<MediaPlayer> resetPlayer)
        : base(bindingContext)
    {
        _resetPlayer = resetPlayer;
    }

    protected override Func<View, IMvxAndroidBindingContext, AnswerViewHolder> BindableViewHolderCreator =>
        (v, c) => new AnswerViewHolder(v, c);

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        base.OnBindViewHolder(holder, position);

        ((AnswerViewHolder)holder).ResetPlayer ??= _resetPlayer;
    }

    public class AnswerViewHolder : BaseViewHolder
    {
        private readonly MaterialCardView _layout;
        private readonly LinearProgressIndicator _progress;
        private readonly ImageView _icon;
        private readonly CircularProgressIndicator _loader;

        private MediaPlayer _player;
        private IDisposable _subscription;

        public AnswerViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _layout = itemView.FindViewById<MaterialCardView>(Resource.Id.video_to_audio_exercise_answer_item_layout);
            _progress = itemView.FindViewById<LinearProgressIndicator>(Resource.Id.video_to_audio_exercise_answer_item_progress);
            var number = itemView.FindViewById<TextView>(Resource.Id.video_to_audio_exercise_answer_item_number);
            _icon = itemView.FindViewById<ImageView>(Resource.Id.video_to_audio_exercise_answer_item_icon);
            _loader = itemView.FindViewById<CircularProgressIndicator>(Resource.Id.video_to_audio_exercise_answer_item_loader);

            _progress.Progress = 0;
            _loader.Visibility = ViewStates.Gone;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AnswerViewHolder, VideoToAudioExerciseAnswer>();

                set.Bind(number)
                    .For(x => x.Text)
                    .To(vm => vm.Number);

                set.Bind(_icon)
                    .For(x => x.ImageTintList)
                    .To(vm => vm.IsSelected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(isSelected =>
                        Application.Context.Resources.GetColorStateList(!isSelected ? Resource.Color.lightPurple : Resource.Color.background)));

                set.Apply();
            });
        }

        public Func<MediaPlayer> ResetPlayer { get; set; }

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
                        _loader.Visibility = ViewStates.Visible;
                        _icon.Visibility = ViewStates.Gone;
                        _layout.StrokeWidth = DimensUtils.DpToPx(Application.Context, 1);
                        _progress.Visibility = ViewStates.Visible;

                        _player = ResetPlayer();

                        _player.SetOnPreparedListener(new MediaPlayerPreparedListener(() =>
                        {
                            _loader.Visibility = ViewStates.Gone;
                            _icon.Visibility = ViewStates.Visible;

                            StartPlayer();
                        }));

                        _player.SetOnCompletionListener(new MediaPlayerCompletionListener(UpdateUi));

                        _player.SetDataSourceAsync(Application.Context, Uri.Parse(ViewModel.AudioUrl))
                            .ContinueWith(_ => _player?.PrepareAsync());

                        return;
                    }

                    // Player will be reset and recreated by another viewholder so not needed to reset or pause it here
                    _layout.StrokeWidth = 0;
                    _progress.Visibility = ViewStates.Invisible;

                    if (x.IsSelected)
                    {
                        _layout.CardBackgroundColor = Application.Context.Resources.GetColorStateList(ViewModel.IsCorrect ? Resource.Color.accent : Resource.Color.alertDanger);
                    }
                })
                .DisposeWith(CompositeDisposable);
        }

        protected override void OnItemViewClick(object sender, EventArgs e)
        {
            if (_player == null)
            {
                // Invoke command instead
                base.OnItemViewClick(sender, e);

                return;
            }

            if (_player.IsPlaying)
            {
                _player.Pause();

                return;
            }

            StartPlayer();
        }

        private void StartPlayer()
        {
            _player.Start();

            Task.Run(
                async () =>
                {
                    while (_player.IsPlaying)
                    {
                        _progress.SetProgress((int)((float)_player.CurrentPosition / _player.Duration * 100), true);

                        await Task.Delay(200);
                    }
                });
        }

        private void UpdateUi()
        {
            _progress.Visibility = _player.IsPlaying ? ViewStates.Visible : ViewStates.Invisible;
            _progress.SetProgress(0, false);
            _layout.StrokeWidth = _player.IsPlaying ? DimensUtils.DpToPx(Application.Context, 1) : 0;
        }
    }
}
