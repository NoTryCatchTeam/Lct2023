using System.Threading.Tasks;
using Lct2023.Definitions;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IDialogService _dialogService;

    public LoginViewModel(
        IUserService userService,
        IUserContext userContext,
        IDialogService dialogService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _userService = userService;
        _userContext = userContext;
        _dialogService = dialogService;

        SignInViaSocialCommand = new MvxAsyncCommand(SignInViaSocialAsync);
        SignInBasicCommand = new MvxAsyncCommand(SignInBasicAsync);
    }

    public IMvxAsyncCommand SignInViaSocialCommand { get; }
    
    public IMvxAsyncCommand SignInBasicCommand { get; }

    public IMvxAsyncCommand SignUpCommand { get; }

    public IMvxAsyncCommand SignInCommand { get; }

    private Task SignInViaSocialAsync()
    {
        return RunSafeTaskAsync(
            async () =>
            {
                await _userService.SignInViaSocialAsync(CancellationToken);

                _dialogService.ShowToast($"{_userContext.User.FirstName} you've signed it");
            });
    }

    private Task SignInBasicAsync()
    {
        return RunSafeTaskAsync(
            async () =>
            {
                await _userService.SignInAsync("dima.ordenov", "qwerty123", CancellationToken);

                _dialogService.ShowToast($"{_userContext.User.FirstName} you've signed it");
            });
    }
}
