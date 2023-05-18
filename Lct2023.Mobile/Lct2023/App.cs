using MvvmCross.ViewModels;

namespace Lct2023;

public class App : MvxApplication
{
    public override void Initialize()
    {
        base.Initialize();

        RegisterCustomAppStart<AppStart>();
    }
}
