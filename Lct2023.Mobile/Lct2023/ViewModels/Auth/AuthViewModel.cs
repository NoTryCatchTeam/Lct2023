using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Lct2023.Definitions.Dtos;
using Lct2023.Definitions.Models;
using Lct2023.Definitions.MvxIntercationResults;
using Lct2023.Definitions.Types;
using Lct2023.Services;
using Lct2023.Services.Implementation;
using Lct2023.ViewModels.Anonymous;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Auth;

public class AuthViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;
    private readonly IDialogService _dialogService;
    private readonly IValidator<SignInFields> _signInValidator;
    private readonly IValidator<SignUpFields> _signUpValidator;

    private readonly MvxInteraction<ValidationInteractionResult> _validationInteractionLocal;

    private AuthViewState _state;

    public AuthViewModel(
        IUserService userService,
        IMediaService mediaService,
        IDialogService dialogService,
        IValidator<SignInFields> signInValidator,
        IValidator<SignUpFields> signUpValidator,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _userService = userService;
        _mediaService = mediaService;
        _dialogService = dialogService;
        _signInValidator = signInValidator;
        _signUpValidator = signUpValidator;

        UploadPhotoCommand = new MvxAsyncCommand(UploadPhotoAsync, () => State == 0);
        SignInCommand = new MvxAsyncCommand(SignInAsync, () => State == 0);
        SignInViaVkCommand = new MvxAsyncCommand(SignInViaVkAsync, () => State == 0);
        SignInAnonymousCommand = new MvxAsyncCommand(SignInAnonymousAsync, () => State == 0);
        SignUpCommand = new MvxAsyncCommand(SignUpAsync, () => State == 0);

        _validationInteractionLocal = new MvxInteraction<ValidationInteractionResult>();
        SignIn = new SignInFields();
        SignUp = new SignUpFields();
    }

    public IMvxAsyncCommand UploadPhotoCommand { get; }

    public IMvxAsyncCommand SignInCommand { get; }

    public IMvxAsyncCommand SignInViaVkCommand { get; }

    public IMvxAsyncCommand SignInAnonymousCommand { get; }

    public IMvxAsyncCommand SignUpCommand { get; }

    public SignInFields SignIn { get; }

    public SignUpFields SignUp { get; }

    public AuthViewState State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    public IMvxInteraction<ValidationInteractionResult> ValidationInteraction => _validationInteractionLocal;

    private async Task UploadPhotoAsync()
    {
        await RunSafeTaskAsync(
            async () =>
            {
                var photo = await _mediaService.ChoosePhotoAsync();

                await using var stream = await photo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                SignUp.PhotoBase64 = Convert.ToBase64String(memoryStream.ToArray());
            },
            _ =>
            {
                SignUp.PhotoBase64 = null;

                return Task.CompletedTask;
            });
    }

    private async Task SignInAsync()
    {
        State |= AuthViewState.SigningIn;

        await RunSafeTaskAsync(
            async () =>
            {
                await _signInValidator.ValidateAndThrowAsync(SignIn, CancellationToken);

                _dialogService.ShowToast("Пробуем авторизоваться");

                await _userService.SignInAsync(SignIn.Email, SignIn.Password, CancellationToken);

                await NavigationService.Navigate<MainTabbedViewModel>();
            },
            ex =>
            {
                switch (ex)
                {
                    case ValidationException validationEx:
                        _validationInteractionLocal.Raise(new ValidationInteractionResult(typeof(SignInFields)) { ValidationResults = validationEx.Errors });

                        _dialogService.ShowToast(validationEx.Errors.First().ErrorMessage);

                        break;
                    default:
                        _dialogService.ShowToast($"Ошибка при авторизации");

                        break;
                }

                return Task.CompletedTask;
            });

        State &= ~AuthViewState.SigningIn;
    }

    private async Task SignInViaVkAsync()
    {
        State |= AuthViewState.SigningInViaVk;

        await RunSafeTaskAsync(
            async () =>
            {
                await _userService.SignInViaSocialAsync(CancellationToken);

                await NavigationService.Navigate<MainTabbedViewModel>();
            },
            ex =>
            {
                if (ex is TaskCanceledException)
                {
                    return Task.CompletedTask;
                }

                _dialogService.ShowToast("Произошла ошибка при попытке авторизоваться через VK");

                return Task.CompletedTask;
            });

        State &= ~AuthViewState.SigningInViaVk;
    }

    private async Task SignInAnonymousAsync()
    {
        if ((await NavigationService.Navigate<AnonymousWarningViewModel, AnonymousWarningViewModel.NavResult>())?.IsContinue != true)
        {
            return;
        }

        await NavigationService.Navigate<MainTabbedViewModel>();
    }

    private async Task SignUpAsync()
    {
        State |= AuthViewState.SigningUp;

        await RunSafeTaskAsync(
            async () =>
            {
                await _signUpValidator.ValidateAndThrowAsync(SignUp, CancellationToken);

                await _userService.SignUpAsync(
                    new CreateUserDto
                    {
                        Email = SignUp.Email,
                        FirstName = SignUp.Name,
                        LastName = SignUp.Surname,
                        Password = SignUp.Password,
                        Photo = SignUp.PhotoBase64,
                        BirthDate = SignUp.Birthday,
                    },
                    CancellationToken);

                await NavigationService.Navigate<MainTabbedViewModel>();
            },
            ex =>
            {
                switch (ex)
                {
                    case ValidationException validationEx:
                        _validationInteractionLocal.Raise(new ValidationInteractionResult(typeof(SignUpFields)) { ValidationResults = validationEx.Errors });

                        _dialogService.ShowToast(validationEx.Errors.First().ErrorMessage);

                        break;
                    default:
                        _dialogService.ShowToast($"Ошибка при регистрации");

                        break;
                }

                return Task.CompletedTask;
            });

        State &= ~AuthViewState.SigningUp;
    }
}
