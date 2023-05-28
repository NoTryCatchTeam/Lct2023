using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.ProfTest;

public class ProfTestFinishViewModel : BaseViewModel
{
    public ProfTestFinishViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }

    public override void Prepare()
    {
        base.Prepare();

        var user = UserContext.User;
        user.IsProfTestFinished = true;

        UserContext.StoreAsync(user);
    }
}
