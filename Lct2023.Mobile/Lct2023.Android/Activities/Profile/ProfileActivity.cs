using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Converters;
using Lct2023.Definitions.Internals;
using Lct2023.ViewModels.Profile;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Square.Picasso;

namespace Lct2023.Android.Activities.Profile
{
    [MvxActivityPresentation]
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProfileActivity : BaseActivity<ProfileViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ProfileActivity);
            Toolbar.Title = "Профиль";

            var avatar = FindViewById<ImageView>(Resource.Id.toolbar_image);
            var username = FindViewById<TextView>(Resource.Id.profile_username);
            var login = FindViewById<TextView>(Resource.Id.profile_login);
            var settingsButton = FindViewById<MaterialButton>(Resource.Id.profile_settings_button);
            var ratingButton = FindViewById<MaterialButton>(Resource.Id.profile_rating_table_button);


            if (ViewModel.UserContext.User?.PhotoUrl is { } photoUrl)
            {
                Picasso.Get()
                    .Load(Uri.Parse(photoUrl))
                    .Placeholder(Resource.Drawable.ic_profile_circle)
                    .Error(Resource.Drawable.ic_profile_circle)
                    .Into(
                        avatar,
                        () => avatar.ImageTintList = null,
                        _ =>
                        {
                        });
            }
            else
            {
                avatar.SetImageResource(Resource.Drawable.ic_profile_circle);
            }

            var set = CreateBindingSet();

            set.Bind(username)
                .For(x => x.Text)
                .To(vm => vm.UserContext.User)
                .WithConversion(new AnyExpressionConverter<User, string>(user => $"{user.FirstName} {user.LastName}"));

            set.Bind(login)
                .For(x => x.Text)
                .To(vm => vm.UserContext.User.Email);

            set.Bind(ratingButton)
                .For(x => x.BindClick())
                .To(vm => vm.RatingTableCommand);

            set.Bind(settingsButton)
                .For(x => x.BindClick())
                .To(vm => vm.SettingsCommand);

            set.Apply();
        }
    }
}

