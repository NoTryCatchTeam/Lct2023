using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Google.Android.Material.Button;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Courses;

[MvxFragmentPresentation]
public class LessonLessonPartFragment : MvxFragment
{
    private LessonViews _views;

    private CourseLessonViewModel _viewModel;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        _viewModel = (CourseLessonViewModel)DataContext;

        var view = this.BindingInflate(Resource.Layout.CourseLessonLessonPartFragment, container, false);

        _views = new LessonViews(
            view.FindViewById<TextView>(Resource.Id.course_lesson_lesson_main_material_description),
            view.FindViewById<PlayerView>(Resource.Id.course_lesson_lesson_video),
            view.FindViewById<MaterialButton>(Resource.Id.course_lesson_lesson_file),
            view.FindViewById<TextView>(Resource.Id.course_lesson_lesson_additional_material_title),
            view.FindViewById<TextView>(Resource.Id.course_lesson_lesson_additional_material_description));

        if (_viewModel.NavigationParameter.LessonItem.Attachment?.IsVideo == true)
        {
            _views.Video.Player = GetVideoPlayer();
        }

        var set = this.CreateBindingSet<LessonLessonPartFragment, CourseLessonViewModel>();

        set.Bind(_views.MainDescription)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Description);

        set.Bind(_views.Video)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Attachment)
            .WithConversion(new AnyExpressionConverter<CourseLessonAttachment, bool>(x => x.IsVideo));

        set.Bind(_views.File)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Attachment)
            .WithConversion(new AnyExpressionConverter<CourseLessonAttachment, bool>(x => !x.IsVideo));

        set.Bind(_views.File)
            .For(x => x.BindClick())
            .To(vm => vm.OpenAttachmentCommand);

        set.Bind(_views.AdditionalDescriptionTitle)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.AdditionalDescription)
            .WithConversion(new AnyExpressionConverter<string, bool>(x => !string.IsNullOrEmpty(x)));

        set.Bind(_views.AdditionalDescription)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.AdditionalDescription)
            .WithConversion(new AnyExpressionConverter<string, bool>(x => !string.IsNullOrEmpty(x)));

        set.Bind(_views.AdditionalDescription)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.AdditionalDescription);

        set.Apply();

        return view;
    }

    public override void OnResume()
    {
        base.OnResume();
        _views.Video?.OnResume();
    }

    public override void OnPause()
    {
        base.OnPause();
        _views.Video?.OnPause();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (_views.Video == null)
        {
            return;
        }

        _views.Video.Player?.Release();
        _views.Video.Player = null;
    }

    private SimpleExoPlayer GetVideoPlayer()
    {
        var videoPlayer = new SimpleExoPlayer.Builder(Activity)
            .Build();

        var mediaSource = new ProgressiveMediaSource.Factory(
                new DefaultDataSourceFactory(Activity, Util.GetUserAgent(Activity, Activity.PackageName)))
            .CreateMediaSource(MediaItem.FromUri(_viewModel.NavigationParameter.LessonItem.Attachment.Url));

        videoPlayer.Prepare(mediaSource);
        videoPlayer.RepeatMode = SimpleExoPlayer.InterfaceConsts.RepeatModeOff;
        videoPlayer.PlayWhenReady = false;
        videoPlayer.Volume = 1;

        return videoPlayer;
    }

    private class LessonViews
    {
        public LessonViews(TextView mainDescription, PlayerView video, MaterialButton file, TextView additionalDescriptionTitle, TextView additionalDescription)
        {
            MainDescription = mainDescription;
            Video = video;
            File = file;
            AdditionalDescriptionTitle = additionalDescriptionTitle;
            AdditionalDescription = additionalDescription;
        }

        public TextView MainDescription { get; }

        public PlayerView Video { get; }

        public MaterialButton File { get; }

        public TextView AdditionalDescriptionTitle { get; }

        public TextView AdditionalDescription { get; }
    }
}
