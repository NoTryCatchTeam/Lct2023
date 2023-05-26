using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.Helpers;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using ReactiveUI;
using Xamarin.Essentials;

namespace Lct2023.Android.Fragments.Courses;

[MvxFragmentPresentation]
public class LessonAnswerPartFragment : BaseFragment
{
    private CourseLessonViewModel _viewModel;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        _viewModel = (CourseLessonViewModel)DataContext;

        var resolutionViews = new ResolutionViews(
            view.FindViewById<MaterialCardView>(Resource.Id.course_lesson_answer_resolution),
            view.FindViewById<ConstraintLayout>(Resource.Id.course_lesson_answer_upload_layout),
            view.FindViewById<TextView>(Resource.Id.course_lesson_answer_upload_text),
            view.FindViewById<ConstraintLayout>(Resource.Id.course_lesson_answer_resolution_uploaded_layout),
            view.FindViewById<TextView>(Resource.Id.course_lesson_answer_resolution_uploaded_layout_name),
            view.FindViewById<TextView>(Resource.Id.course_lesson_answer_resolution_uploaded_layout_size));

        (TextView Header, TextView Description, TextInputLayout Layout, TextInputEditText Text) comment = (
            view.FindViewById<TextView>(Resource.Id.course_lesson_answer_comment_header),
            view.FindViewById<TextView>(Resource.Id.course_lesson_answer_comment_description),
            view.FindViewById<TextInputLayout>(Resource.Id.course_lesson_answer_comment_text_layout),
            view.FindViewById<TextInputEditText>(Resource.Id.course_lesson_answer_comment_text_value));

        (MaterialButton Button, CircularProgressIndicator Loader) send = (
            view.FindViewById<MaterialButton>(Resource.Id.course_lesson_answer_send),
            view.FindViewById<CircularProgressIndicator>(Resource.Id.course_lesson_answer_loader));

        var conversation = view.FindViewById<MvxRecyclerView>(Resource.Id.course_lesson_answer_conversation);

        var conversationAdapter = new LessonAnswerConversationAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.course_lesson_conversation_item),
        };

        conversation.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        conversation.SetAdapter(conversationAdapter);
        conversation.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Vertical));

        var set = this.CreateBindingSet<LessonAnswerPartFragment, CourseLessonViewModel>();

        set.Bind(resolutionViews.View)
            .For(x => x.BindClick())
            .To(vm => vm.NavigationParameter.LessonItem.Status)
            .WithConversion(new AnyExpressionConverter<CourseLessonStatus, ICommand>(x =>
                x == CourseLessonStatus.Available ? _viewModel.UploadResolutionCommand : _viewModel.OpenResolutionCommand));

        set.Bind(resolutionViews.UploadText)
            .For(x => x.Text)
            .To(vm => vm.PickedMedia)
            .WithConversion(new AnyExpressionConverter<FileResult, string>(x =>
                x != null ? x.FileName : "Нажмите для загрузки домашнего задания"));

        set.Bind(resolutionViews.ResolutionName)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Resolution.FileName);

        set.Bind(resolutionViews.ResolutionSize)
            .For(x => x.Text)
            .To(vm => vm.NavigationParameter.LessonItem.Resolution.FileSize)
            .WithConversion(new AnyExpressionConverter<long, string>(x => x.GetHumanReadableString()));

        set.Bind(comment.Header)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Status)
            .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.Available));

        set.Bind(comment.Description)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Status)
            .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.Available));

        set.Bind(comment.Layout)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Status)
            .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.Available));

        set.Bind(comment.Text)
            .For(x => x.Text)
            .To(vm => vm.AnswerComment);

        set.Bind(send.Button)
            .For(x => x.BindClick())
            .To(vm => vm.SendAnswerCommand);

        set.Bind(send.Button)
            .For(x => x.BindVisible())
            .To(vm => vm.NavigationParameter.LessonItem.Status)
            .WithConversion(new AnyExpressionConverter<CourseLessonStatus, bool>(x => x == CourseLessonStatus.Available));

        set.Bind(send.Loader)
            .For(x => x.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<CourseLessonViewState, bool>(x => x.HasFlag(CourseLessonViewState.SendingAnswer)));

        set.Bind(conversationAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.NavigationParameter.LessonItem.ConversationItems);

        set.Apply();

        _viewModel.WhenAnyValue(
                x => x.NavigationParameter.LessonItem.Resolution,
                x => x.NavigationParameter.LessonItem.Status)
            .Subscribe(((CourseLessonResolution Resolution, CourseLessonStatus Status) x) =>
            {
                var isSolvedAndNoResolution = x.Status != CourseLessonStatus.Available && x.Resolution == null;

                resolutionViews.View.Visibility = isSolvedAndNoResolution ? ViewStates.Gone : ViewStates.Visible;

                var shouldUpload = x is { Status: CourseLessonStatus.Available, Resolution: null };

                resolutionViews.UploadLayout.Visibility = shouldUpload ? ViewStates.Visible : ViewStates.Gone;
                resolutionViews.UploadedLayout.Visibility = !shouldUpload ? ViewStates.Visible : ViewStates.Gone;
            })
            .DisposeWith(CompositeDisposable);

        return view;
    }

    public override void OnResume()
    {
        base.OnResume();

        try
        {
            Activity.FindViewById<ConstraintLayout>(Resource.Id.course_lesson_layout).RequestLayout();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    protected override int GetLayoutId() => Resource.Layout.CourseLessonAnswerPartFragment;

    private class ResolutionViews
    {
        public ResolutionViews(MaterialCardView view, ConstraintLayout uploadLayout, TextView uploadText, ConstraintLayout uploadedLayout, TextView resolutionName, TextView resolutionSize)
        {
            View = view;
            UploadLayout = uploadLayout;
            UploadText = uploadText;
            UploadedLayout = uploadedLayout;
            ResolutionName = resolutionName;
            ResolutionSize = resolutionSize;
        }

        public MaterialCardView View { get; }

        public ConstraintLayout UploadLayout { get; }

        public TextView UploadText { get; }

        public ConstraintLayout UploadedLayout { get; }

        public TextView ResolutionName { get; }

        public TextView ResolutionSize { get; }
    }
}
