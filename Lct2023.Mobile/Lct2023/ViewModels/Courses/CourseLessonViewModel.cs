using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Lct2023.Definitions.Models;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using Lct2023.ViewModels.Common;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonViewModel : BaseViewModel<CourseLessonViewModel.NavParameter>
{
    private readonly IDialogService _dialogService;
    private readonly IMediaService _mediaService;
    private readonly IPlatformFileViewer _platformFileViewer;

    private FileResult _pickedMedia;

    public CourseLessonViewModel(
        IDialogService dialogService,
        IMediaService mediaService,
        IPlatformFileViewer platformFileViewer,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _dialogService = dialogService;
        _mediaService = mediaService;
        _platformFileViewer = platformFileViewer;

        UploadResolutionCommand = new MvxAsyncCommand(UploadResolutionAsync);
        OpenResolutionCommand = new MvxAsyncCommand(OpenResolutionAsync);
        SendAnswerCommand = new MvxAsyncCommand(SendAnswerAsync);
        OpenAttachmentCommand = new MvxAsyncCommand(() =>
            NavigationService.Navigate<WebViewModel, WebViewModel.NavParameter>(new WebViewModel.NavParameter(NavigationParameter.LessonItem.Attachment.Url)));
    }

    public IMvxAsyncCommand UploadResolutionCommand { get; }

    public IMvxAsyncCommand OpenResolutionCommand { get; }

    public IMvxAsyncCommand SendAnswerCommand { get; }

    public IMvxAsyncCommand OpenAttachmentCommand { get; }

    public FileResult PickedMedia
    {
        get => _pickedMedia;
        set => SetProperty(ref _pickedMedia, value);
    }

    public string AnswerComment { get; set; }

    public CourseLessonViewState State { get; set; }

    private Task UploadResolutionAsync() =>
        RunSafeTaskAsync(
            async () =>
            {
                PickedMedia = await _mediaService.ChooseMediaAsync();
            });

    private Task OpenResolutionAsync() =>
        RunSafeTaskAsync(
            () => _platformFileViewer.OpenFileAsync(NavigationParameter.LessonItem.Resolution.FullPath));

    private async Task SendAnswerAsync()
    {
        State |= CourseLessonViewState.SendingAnswer;

        await RunSafeTaskAsync(
            async () =>
            {
                await Task.Delay(1000);

                // await uploadfile
                var newConversationItem = new CourseLessonConversation
                {
                    // TODO Get user from usercontext
                    Author = new ConversationAuthor
                    {
                        Name = "qwerty qweqwtr",
                        AvatarUrl = "https://sm.ign.com/ign_ap/cover/a/avatar-gen/avatar-generations_hugw.jpg",
                        IsStudent = true,
                    },
                    Text = AnswerComment ?? "Задание выполнено",
                };

                NavigationParameter.LessonItem.Status = CourseLessonStatus.WaitingForReview;

                if (_pickedMedia != null)
                {
                    NavigationParameter.LessonItem.Resolution = new CourseLessonResolution(_pickedMedia);
                }

                NavigationParameter.LessonItem.ConversationItems.Add(newConversationItem);
            });

        State &= ~CourseLessonViewState.SendingAnswer;
    }

    public class NavParameter
    {
        public NavParameter(CourseLessonItem lessonItem)
        {
            LessonItem = lessonItem;
        }

        public CourseLessonItem LessonItem { get; }
    }
}

[Flags]
public enum CourseLessonViewState
{
    SendingAnswer = 1 << 0,
}
