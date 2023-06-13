using System.Threading;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestFinishViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    public ProfTestFinishViewModel(
        IUserService userService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _userService = userService;
    }

    public override void ViewCreated()
    {
        base.ViewCreated();

        var user = UserContext.User;
        user.IsProfTestFinished = true;

        UserContext.StoreAsync(user)
            .ContinueWith(_ =>
                RunSafeTaskAsync(() => _userService.UpdateRatingAsync(50, CancellationToken.None)));
    }
}
