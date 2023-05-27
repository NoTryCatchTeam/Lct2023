using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Lct2023.Android.Callbacks;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels.Auth;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.Auth;

[MvxFragmentPresentation]
public class AuthSignUpFragment : BaseFragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        var parentMotionLayout = Activity.FindViewById<MotionLayout>(Resource.Id.auth_layout);
        var signUp = new SignUp(
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signup_email_value),
            view.FindViewById<TextInputLayout>(Resource.Id.auth_signup_username_layout),
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signup_username_value),
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signup_password_value),
            view.FindViewById<TextInputEditText>(Resource.Id.auth_signup_repeat_password_value),
            view.FindViewById<MaterialButton>(Resource.Id.auth_signup_sign_up));

        var clickListener = new DefaultClickListener(v =>
        {
            switch (v.Id)
            {
                case Resource.Id.auth_signup_sign_up:
                    parentMotionLayout.TransitionToState(Resource.Id.signup);
                    Activity.HideKeyboard();

                    break;
            }
        });

        signUp.Next.SetOnClickListener(clickListener);

        var set = this.CreateBindingSet<AuthSignUpFragment, AuthViewModel>();

        set.Bind(signUp.EmailValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Email)
            .OneWayToSource();

        set.Bind(signUp.UsernameValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Username)
            .OneWay();

        set.Bind(signUp.PasswordValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Password)
            .OneWayToSource();

        set.Bind(signUp.RepeatPasswordValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.RepeatPassword)
            .OneWayToSource();

        set.Apply();

        return view;
    }

    protected override int GetLayoutId() => Resource.Layout.AuthSignUpFragment;

    private class SignUp
    {
        public SignUp(
            TextInputEditText emailValue,
            TextInputLayout usernameLayout,
            TextInputEditText usernameValue,
            TextInputEditText passwordValue,
            TextInputEditText repeatPasswordValue,
            MaterialButton next)
        {
            EmailValue = emailValue;
            UsernameLayout = usernameLayout;
            UsernameValue = usernameValue;
            PasswordValue = passwordValue;
            RepeatPasswordValue = repeatPasswordValue;
            Next = next;
        }

        public TextInputEditText EmailValue { get; }

        public TextInputLayout UsernameLayout { get; }

        public TextInputEditText UsernameValue { get; }

        public TextInputEditText PasswordValue { get; }

        public TextInputEditText RepeatPasswordValue { get; }

        public MaterialButton Next { get; }
    }
}
