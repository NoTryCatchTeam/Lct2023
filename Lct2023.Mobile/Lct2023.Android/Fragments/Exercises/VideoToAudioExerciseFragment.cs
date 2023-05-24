using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Exercises;

[MvxFragmentPresentation]
public class VideoToAudioExerciseFragment : MvxFragment, IPlayersFragment
{
    private MediaPlayer _audioPlayer;
    private PlayerView _videoView;

    private VideoToAudioExerciseItem _viewModel;

    public void ReleasePlayers()
    {
        _videoView.Player?.Release();
        _videoView.Player = null;

        _audioPlayer?.Release();
        _audioPlayer = null;
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        _viewModel = (VideoToAudioExerciseItem)DataContext;

        var view = this.BindingInflate(Resource.Layout.VideoToAudioExerciseFragment, container, false);

        var question = view.FindViewById<TextView>(Resource.Id.video_to_audio_exercise_question);
        _videoView = view.FindViewById<PlayerView>(Resource.Id.video_to_audio_exercise_video);
        var answers = view.FindViewById<MvxRecyclerView>(Resource.Id.video_to_audio_exercise_answers_list);
        var comment = view.FindViewById<TextView>(Resource.Id.video_to_audio_exercise_answers_comment_description);

        _videoView.Player = GetVideoPlayer();

        var answersAdapter = new VideoToAudioExerciseAnswersAdapter(
            (IMvxAndroidBindingContext)BindingContext,
            () =>
            {
                _audioPlayer ??= new MediaPlayer();
                _audioPlayer.Reset();

                return _audioPlayer;
            })
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.video_to_audio_exercise_answer_item),
        };

        answers.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        answers.SetAdapter(answersAdapter);
        answers.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical));

        var set = this.CreateBindingSet<VideoToAudioExerciseFragment, VideoToAudioExerciseItem>();

        set.Bind(question)
            .For(x => x.Text)
            .To(vm => vm.Question);

        set.Bind(answersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.Answers);

        set.Bind(answersAdapter)
            .For(x => x.ItemClick)
            .To(vm => vm.AnswerTapCommand);

        set.Bind(comment)
            .For(x => x.Text)
            .To(vm => vm.DescriptionOfCorrectness);

        set.Bind(comment)
            .For(x => x.BindVisible())
            .To(vm => vm.IsCorrect)
            .WithConversion(new AnyExpressionConverter<bool, bool>(x => !x));

        set.Apply();

        return view;
    }

    public override void OnResume()
    {
        base.OnResume();
        _videoView?.OnResume();
    }

    public override void OnPause()
    {
        base.OnPause();
        _videoView?.OnPause();
        _audioPlayer?.Pause();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_videoView == null)
        {
            return;
        }

        ReleasePlayers();
    }

    private SimpleExoPlayer GetVideoPlayer()
    {
        var videoPlayer = new SimpleExoPlayer.Builder(Activity)
            .Build();

        var mediaSource = new ProgressiveMediaSource.Factory(
                new DefaultDataSourceFactory(Activity, Util.GetUserAgent(Activity, Activity.PackageName)))
            .CreateMediaSource(MediaItem.FromUri(_viewModel.VideoUrl));

        videoPlayer.Prepare(mediaSource);
        videoPlayer.RepeatMode = SimpleExoPlayer.InterfaceConsts.RepeatModeOff;
        videoPlayer.PlayWhenReady = false;
        videoPlayer.Volume = 1;

        return videoPlayer;
    }

    private MediaPlayer GetAudioPlayer()
    {
        var player = new MediaPlayer();

        return player;
    }
}
