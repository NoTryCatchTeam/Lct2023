using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Lct2023.Business.RestServices.Auth;
using Lct2023.Definitions.Dtos;
using Lct2023.Definitions.MvxIntercationResults;
using Lct2023.Services;
using Lct2023.Services.Implementation;
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

                await _userService.SignInAsync(SignIn.Email, SignIn.Password, CancellationToken);
            },
            ex =>
            {
                switch (ex)
                {
                    case ValidationException validationEx:
                        _validationInteractionLocal.Raise(new ValidationInteractionResult(typeof(SignInFields)) { ValidationResults = validationEx.Errors });

                        _dialogService.ShowToast(validationEx.Errors.First().ErrorMessage);

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

    private Task SignInAnonymousAsync() =>
        NavigationService.Navigate<MainTabbedViewModel>();

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
                    },
                    CancellationToken);
            },
            ex =>
            {
                switch (ex)
                {
                    case ValidationException validationEx:
                        _validationInteractionLocal.Raise(new ValidationInteractionResult(typeof(SignUpFields)) { ValidationResults = validationEx.Errors });

                        break;
                }

                return Task.CompletedTask;
            });

        State &= ~AuthViewState.SigningUp;
    }
}

[Flags]
public enum AuthViewState
{
    SigningIn = 1 << 0,

    SigningInViaVk = 1 << 1,

    SigningUp = 1 << 2,
}

public class SignInFields
{
    public string Email { get; set; }

    public string Password { get; set; }
}

public class SignUpFields : MvxNotifyPropertyChanged
{
    private string _email;
    private string _username;
    private DateTimeOffset? _birthday;

    public string Email
    {
        get => _email;
        set
        {
            if (!SetProperty(ref _email, value))
            {
                return;
            }

            Username = _email.Split("@").FirstOrDefault();
        }
    }

    public string Username
    {
        get => _username;
        private set => SetProperty(ref _username, value);
    }

    public string Password { get; set; }

    public string RepeatPassword { get; set; }

    public string PhotoBase64 { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTimeOffset? Birthday
    {
        get => _birthday;
        set => SetProperty(ref _birthday, value);
    }
}
