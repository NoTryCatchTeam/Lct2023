using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Fragments.Exercises;

[MvxFragmentPresentation]
public class AudioToPictureExerciseFragment : MvxFragment, IPlayersFragment
{
    private PlayViews _playViews;

    private AudioToPictureExerciseItem _viewModel;
    private MediaPlayer _player;

    public void ReleasePlayers()
    {
        _player?.Release();
        _player = null;
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        _viewModel = (AudioToPictureExerciseItem)DataContext;

        var view = this.BindingInflate(Resource.Layout.AudioToPictureExerciseFragment, container, false);
        var question = view.FindViewById<TextView>(Resource.Id.audio_to_picture_exercise_question);

        _playViews = new PlayViews(
            view.FindViewById<MaterialCardView>(Resource.Id.audio_to_picture_exercise_question_layout),
            view.FindViewById<LinearProgressIndicator>(Resource.Id.audio_to_picture_exercise_question_progress),
            view.FindViewById<ImageView>(Resource.Id.audio_to_picture_exercise_question_play_icon),
            view.FindViewById<TextView>(Resource.Id.audio_to_picture_exercise_question_play_text),
            view.FindViewById<CircularProgressIndicator>(Resource.Id.audio_to_picture_exercise_question_play_loader));

        var answers = view.FindViewById<MvxRecyclerView>(Resource.Id.audio_to_picture_exercise_answers_list);

        _player = GetAudioPlayer();

        _playViews.Progress.Progress = 0;
        _playViews.Icon.Visibility = ViewStates.Invisible;
        _playViews.Text.Visibility = ViewStates.Invisible;
        _playViews.View.Clickable = false;
        _playViews.View.SetOnClickListener(new DefaultClickListener(_ =>
        {
            if (_player.IsPlaying)
            {
                PausePlayer();

                return;
            }

            var isStartedFromBeginning = _player.CurrentPosition == 0;

            _player.Start();
            UpdateUi(isStartedFromBeginning);

            Task.Run(
                async () =>
                {
                    while (_player.IsPlaying)
                    {
                        _playViews.Progress.SetProgress((int)((float)_player.CurrentPosition / _player.Duration * 100), true);

                        await Task.Delay(200);
                    }
                });
        }));

        var answersAdapter = new AudioToPictureExerciseAnswersAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.audio_to_picture_exercise_answer_item),
        };

        answers.SetLayoutManager(new MvxGuardedGridLayoutManager(Activity, 2) { Orientation = LinearLayoutManager.Vertical });
        answers.SetAdapter(answersAdapter);
        answers.AddItemDecoration(new GridItemDecoration(2, DimensUtils.DpToPx(Activity, 16)));

        var set = this.CreateBindingSet<AudioToPictureExerciseFragment, AudioToPictureExerciseItem>();

        set.Bind(question)
            .For(x => x.Text)
            .To(vm => vm.Question);

        set.Bind(answersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.Answers);

        set.Bind(answersAdapter)
            .For(x => x.ItemClick)
            .To(vm => vm.AnswerTapCommand);

        set.Apply();

        return view;
    }

    public override void OnPause()
    {
        base.OnPause();
        PausePlayer();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_player == null)
        {
            return;
        }

        ReleasePlayers();
    }

    private void UpdateUi(bool isStartedFromBeginning)
    {
        _playViews.Progress.Visibility = _player.IsPlaying ? ViewStates.Visible : ViewStates.Invisible;
        _playViews.Text.Text = _player.IsPlaying ? "Остановить воспроизведение" : "Прослушать отрывок";
        _playViews.Text.SetTextColor(Application.Context.Resources.GetColorStateList(_player.IsPlaying ? Resource.Color.lightPurple : Resource.Color.textPrimary, null));
        _playViews.View.StrokeWidth = _player.IsPlaying ? DimensUtils.DpToPx(Activity, 1) : 0;

        if (isStartedFromBeginning)
        {
            _playViews.Progress.SetProgress(0, false);
        }
    }

    private void PausePlayer()
    {
        _playViews.Text.Text = "Продолжить воспроизведение";
        _player?.Pause();
    }

    private MediaPlayer GetAudioPlayer()
    {
        var player = new MediaPlayer();
        player.SetOnPreparedListener(new MediaPlayerPreparedListener(() =>
        {
            _playViews.Icon.Visibility = ViewStates.Visible;
            _playViews.Text.Visibility = ViewStates.Visible;
            _playViews.Loader.Visibility = ViewStates.Gone;

            _playViews.View.Clickable = true;
        }));

        player.SetOnCompletionListener(new MediaPlayerCompletionListener(() => UpdateUi(true)));

        player.SetDataSourceAsync(Activity, Uri.Parse(_viewModel.AudioUrl))
            .ContinueWith(_ => player.PrepareAsync());

        return player;
    }

    private class PlayViews
    {
        public PlayViews(MaterialCardView view, LinearProgressIndicator progress, ImageView icon, TextView text, CircularProgressIndicator loader)
        {
            View = view;
            Progress = progress;
            Icon = icon;
            Text = text;
            Loader = loader;
        }

        public MaterialCardView View { get; }

        public LinearProgressIndicator Progress { get; }

        public ImageView Icon { get; }

        public TextView Text { get; }

        public CircularProgressIndicator Loader { get; }
    }
}
