using System;
using System.Reactive.Disposables;
using Android.OS;
using Android.Views;
using DynamicData.Binding;
using Google.Android.Material.Button;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.Definitions.Types;
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

        var signIn = new SignIn(
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signin_email_value),
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signin_password_value),
            view.FindViewById<MaterialButton>(Resource.Id.auth_signin_sing_in),
            view.FindViewById<CircularProgressIndicator>(Resource.Id.auth_signin_sing_in_loader),
            view.FindViewById<MaterialButton>(Resource.Id.auth_signin_vk),
            view.FindViewById<CircularProgressIndicator>(Resource.Id.auth_signin_singin_vk_loader));

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

        // TODO For some reason can't change visibility of this ui elements. Even through straight invokes and UI thread
        set.Bind(signIn.SignInLoader)
            .For(x => x.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<AuthViewState, bool>(x => x.HasFlag(AuthViewState.SigningIn)));

        set.Bind(signIn.SignInViaVkLoader)
            .For(x => x.BindVisible())
            .To(vm => vm.State)
            .WithConversion(new AnyExpressionConverter<AuthViewState, bool>(x => x.HasFlag(AuthViewState.SigningInViaVk)));

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
            CircularProgressIndicator signInLoader,
            MaterialButton signInViaVk,
            CircularProgressIndicator signInViaVkLoader)
        {
            EmailValue = emailValue;
            PasswordValue = passwordValue;
            SignInButton = signInButton;
            SignInLoader = signInLoader;
            SignInViaVk = signInViaVk;
            SignInViaVkLoader = signInViaVkLoader;
        }

        public TextInputEditText EmailValue { get; }

        public TextInputEditText PasswordValue { get; }

        public MaterialButton SignInButton { get; }

        public CircularProgressIndicator SignInLoader { get; }

        public MaterialButton SignInViaVk { get; }

        public CircularProgressIndicator SignInViaVkLoader { get; }
    }
}
