using Android.App;
using Android.OS;
using Google.Android.Material.Button;
using Lct2023.ViewModels.Auth;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Activities.Auth;

[MvxActivityPresentation]
[Activity]
public class LoginActivity : BaseActivity<LoginViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.LoginActivity);

        var signInVk = FindViewById<MaterialButton>(Resource.Id.login_vk_sign_in);
        var signInBasic = FindViewById<MaterialButton>(Resource.Id.login_basic_sign_in);

        var set = CreateBindingSet();

        set.Bind(signInVk)
            .For(x => x.BindClick())
            .To(vm => vm.SignInViaSocialCommand);

        set.Bind(signInBasic)
            .For(x => x.BindClick())
            .To(vm => vm.SignInBasicCommand);

        set.Apply();
    }
}
