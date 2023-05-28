using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.Tabs;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Callbacks;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.Definitions.MvxIntercationResults;
using Lct2023.ViewModels.Auth;
using MvvmCross.Base;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Lct2023.Android.Activities.Auth;

[MvxActivityPresentation]
[Activity(
    ScreenOrientation = ScreenOrientation.Portrait,
    WindowSoftInputMode = SoftInput.AdjustPan)]
public class AuthActivity : BaseActivity<AuthViewModel>
{
    private IMvxInteraction<ValidationInteractionResult> _validationInteraction;
    private DefaultBackPressedCallback _defaultBackPressedCallback;

    public IMvxInteraction<ValidationInteractionResult> ValidationInteraction
    {
        get => _validationInteraction;
        set
        {
            if (_validationInteraction != null)
            {
                _validationInteraction.Requested -= ValidationInteractionRequested;
            }

            _validationInteraction = value;

            _validationInteraction.Requested += ValidationInteractionRequested;
        }
    }

    private void ValidationInteractionRequested(object sender, MvxValueEventArgs<ValidationInteractionResult> e)
    {
    }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.AuthActivity);

        var parent = FindViewById<MotionLayout>(Resource.Id.auth_layout);

        var authContainerLayout = FindViewById<MaterialCardView>(Resource.Id.auth_motion_auth_container_layout);
        var authTabs = FindViewById<TabLayout>(Resource.Id.auth_tabs);
        var viewPager = FindViewById<ViewPager2>(Resource.Id.auth_view_pager);

        var signUp = new SignUp(
            FindViewById<MaterialCardView>(Resource.Id.auth_signup_2_photo_layout),
            FindViewById<ConstraintLayout>(Resource.Id.auth_signup_2_photo_upload),
            FindViewById<ImageView>(Resource.Id.auth_signup_2_photo),
            FindViewById<TextInputEditText>(Resource.Id.auth_signup_2_name_value),
            FindViewById<TextInputEditText>(Resource.Id.auth_signup_2_surname_value),
            FindViewById<MaterialCardView>(Resource.Id.auth_signup_2_birthday_picker),
            FindViewById<TextView>(Resource.Id.auth_signup_2_birthday_value),
            FindViewById<MaterialButton>(Resource.Id.auth_signup_finish),
            FindViewById<MaterialButton>(Resource.Id.auth_signup_back));

        var anonymous = FindViewById<MaterialButton>(Resource.Id.auth_anonymous);

        _defaultBackPressedCallback = new DefaultBackPressedCallback(
            true,
            callback =>
            {
                if (parent.CurrentState == Resource.Id.signup)
                {
                    parent.TransitionToState(Resource.Id.end);
                }
                else if (viewPager.CurrentItem == 1)
                {
                    viewPager.SetCurrentItem(0, true);
                }
                else
                {
                    callback.Enabled = false;
                }
            });

        OnBackPressedDispatcher.AddCallback(_defaultBackPressedCallback);

        var authViewPagerAdapter = new AuthViewPagerAdapter(this, ViewModel);

        viewPager.UserInputEnabled = false;
        viewPager.Adapter = authViewPagerAdapter;
        viewPager.OffscreenPageLimit = 1;
        viewPager.RegisterOnPageChangeCallback(new AuthViewPagerPageCallback(viewPager, authContainerLayout, _defaultBackPressedCallback));

        new TabLayoutMediator(
                authTabs,
                viewPager,
                false,
                true,
                new DefaultTabConfigurationStrategy(
                    (tab, position) =>
                    {
                        tab.SetText(position switch
                        {
                            0 => "Вход",
                            1 => "Регистрация",
                            _ => null,
                        });
                    }))
            .Attach();

        var birthdayPickerDialog = GetBirthdayPickerDialog();

        var clickListener = new DefaultClickListener(v =>
        {
            switch (v.Id)
            {
                case Resource.Id.auth_signup_back:
                    parent.TransitionToState(Resource.Id.end);
                    this.HideKeyboard();

                    break;
                case Resource.Id.auth_signup_2_birthday_picker:
                    birthdayPickerDialog.Show();

                    break;
            }
        });

        signUp.Back.SetOnClickListener(clickListener);
        signUp.BirthdayLayout.SetOnClickListener(clickListener);

        var set = CreateBindingSet();

        set.Bind(signUp.UploadLayout)
            .For(x => x.BindClick())
            .To(vm => vm.UploadPhotoCommand);

        set.Bind(signUp.UploadInfoLayout)
            .For(x => x.BindVisible())
            .To(vm => vm.SignUp.PhotoBase64)
            .WithConversion(new AnyExpressionConverter<string, bool>(string.IsNullOrEmpty));

        set.Bind(signUp.Photo)
            .For(x => x.BindVisible())
            .To(vm => vm.SignUp.PhotoBase64)
            .WithConversion(new AnyExpressionConverter<string, bool>(x => !string.IsNullOrEmpty(x)));

        set.Bind(signUp.Photo)
            .For(x => x.BindBitmap())
            .To(vm => vm.SignUp.PhotoBase64)
            .WithConversion(new AnyExpressionConverter<string, Bitmap>(x =>
            {
                if (x == null)
                {
                    return null;
                }

                var decodedString = Base64.Decode(x, Base64Flags.Default);

                return BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
            }));

        set.Bind(signUp.NameValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Name)
            .OneWayToSource();

        set.Bind(signUp.SurnameValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Surname)
            .OneWayToSource();

        set.Bind(signUp.BirthdayValue)
            .For(x => x.Text)
            .To(vm => vm.SignUp.Birthday)
            .WithConversion(new AnyExpressionConverter<DateTimeOffset?, string>(x => x == null ? "xx.xx.xxxx" : x.Value.ToString("dd.MM.yyyy")))
            .OneWay();

        set.Bind(signUp.Finish)
            .For(x => x.BindClick())
            .To(vm => vm.SignUpCommand);

        set.Bind(anonymous)
            .For(x => x.BindClick())
            .To(vm => vm.SignInAnonymousCommand);

        set.Bind(this)
            .For(x => x.ValidationInteraction)
            .To(vm => vm.ValidationInteraction)
            .OneWay();

        set.Apply();
    }

    private DatePickerDialog GetBirthdayPickerDialog()
    {
        var now = DateTimeOffset.UtcNow;
        var picker = new DatePickerDialog(
            this,
            Resource.Style.AppTheme_DatePickerDialog,
            (_, e) => ViewModel.SignUp.Birthday = new DateTimeOffset(e.Date),
            now.Year,
            now.Month,
            now.Day);
        
        return picker;
    }

    private class SignUp
    {
        public SignUp(
            MaterialCardView uploadLayout,
            ConstraintLayout uploadInfoLayout,
            ImageView photo,
            TextInputEditText nameValue,
            TextInputEditText surnameValue,
            MaterialCardView birthdayLayout,
            TextView birthdayValue,
            MaterialButton finish,
            MaterialButton back)
        {
            UploadLayout = uploadLayout;
            UploadInfoLayout = uploadInfoLayout;
            Photo = photo;
            NameValue = nameValue;
            SurnameValue = surnameValue;
            BirthdayLayout = birthdayLayout;
            BirthdayValue = birthdayValue;
            Finish = finish;
            Back = back;
        }

        public MaterialCardView UploadLayout { get; }

        public ConstraintLayout UploadInfoLayout { get; }

        public ImageView Photo { get; }

        public TextInputEditText NameValue { get; }

        public TextInputEditText SurnameValue { get; }

        public MaterialCardView BirthdayLayout { get; }

        public TextView BirthdayValue { get; }

        public MaterialButton Finish { get; }

        public MaterialButton Back { get; }
    }
}
