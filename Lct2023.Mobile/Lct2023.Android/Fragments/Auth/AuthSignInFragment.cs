using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Motion.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels.Auth;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.Auth;

[MvxFragmentPresentation]
public class AuthSignInFragment : BaseFragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var viewModel = (AuthViewModel)DataContext;

        var parentMotionLayout = Activity.FindViewById<MotionLayout>(Resource.Id.auth_layout);
        var signIn = new SignIn(
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signin_email_value),
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signin_password_value),
            view.FindViewById<MaterialButton>(Resource.Id.auth_signin_sing_in),
            view.FindViewById<MaterialButton>(Resource.Id.auth_signin_vk));

        var clickListener = new DefaultClickListener(v =>
        {
            Activity.HideKeyboard();

            switch (v.Id)
            {
                case Resource.Id.auth_signin_sing_in:
                    viewModel.SignInCommand.ExecuteAsync();

                    break;
                case Resource.Id.auth_signin_vk:
                    viewModel.SignInViaVkCommand.ExecuteAsync();

                    break;
            }
        });

        signIn.SignInButton.SetOnClickListener(clickListener);

        signIn.SignInViaVk.SetOnClickListener(clickListener);

        var set = this.CreateBindingSet<AuthSignInFragment, AuthViewModel>();

        set.Bind(signIn.EmailValue)
            .For(x => x.Text)
            .To(vm => vm.SignIn.Email)
            .OneWayToSource();

        set.Bind(signIn.PasswordValue)
            .For(x => x.Text)
            .To(vm => vm.SignIn.Password)
            .OneWayToSource();

        // set.Bind(signIn.SignInButton)
        //     .For(x => x.BindClick())
        //     .To(vm => vm.SignInCommand);
        //
        // set.Bind(signIn.SignInViaVk)
        //     .For(x => x.BindClick())
        //     .To(vm => vm.SignInViaVkCommand);

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.AuthSignInFragment;

    private class SignIn
    {
        public SignIn(
            TextInputEditText emailValue,
            TextInputEditText passwordValue,
            MaterialButton signInButton,
            MaterialButton signInViaVk)
        {
            EmailValue = emailValue;
            PasswordValue = passwordValue;
            SignInButton = signInButton;
            SignInViaVk = signInViaVk;
        }

        public TextInputEditText EmailValue { get; }

        public TextInputEditText PasswordValue { get; }

        public MaterialButton SignInButton { get; }

        public MaterialButton SignInViaVk { get; }
    }
}
