using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.Definitions.Internals;
using Lct2023.ViewModels.Profile;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Target;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Square.Picasso;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Activities.Profile;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class ProfileActivity : BaseActivity<ProfileViewModel>
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.ProfileActivity);

        Toolbar.Title = "Профиль";

        var parent = FindViewById<ConstraintLayout>(Resource.Id.profile_layout);
        var scroll = FindViewById<NestedScrollView>(Resource.Id.profile_scroll);
        var toolbarSubview = FindViewById<ConstraintLayout>(Resource.Id.profile_toolbar_subview);
        var avatar = FindViewById<ImageView>(Resource.Id.toolbar_image);
        var username = FindViewById<TextView>(Resource.Id.profile_username);
        var login = FindViewById<TextView>(Resource.Id.profile_login);
        var ratingCounter = FindViewById<TextView>(Resource.Id.profile_points_text);
        var rewards = FindViewById<MvxRecyclerView>(Resource.Id.profile_daily_bonus_collection);
        var rate = FindViewById<MaterialCardView>(Resource.Id.profile_rate_frame);
        var achievements = FindViewById<MvxRecyclerView>(Resource.Id.profile_achievements_collection);
        var friends = FindViewById<MvxRecyclerView>(Resource.Id.profile_friends_collection);
        var allCourses = FindViewById<MaterialButton>(Resource.Id.profile_all_courses_button);

        _ = new ScrollWithOverlayViewMediator(parent, scroll, toolbarSubview);

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

        var rewardsAdapter = new ProfileRewardsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.profile_daily_reward_item),
        };

        rewards.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
        rewards.SetAdapter(rewardsAdapter);
        rewards.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(this, 4), LinearLayoutManager.Horizontal));

        var achievementsAdapter = new ProfileAchievementsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.achievement_item),
        };

        achievements.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
        achievements.SetAdapter(achievementsAdapter);
        achievements.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(this, 8), LinearLayoutManager.Horizontal));

        var friendsAdapter = new ProfileFriendsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.profile_friend_item),
        };

        friends.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Vertical });
        friends.SetAdapter(friendsAdapter);
        friends.AddItemDecoration(new ColoredDividerItemDecoration(this, LinearLayoutManager.Vertical));

        var set = CreateBindingSet();

        set.Bind(username)
            .For(x => x.Text)
            .To(vm => vm.UserContext.User)
            .WithConversion(new AnyExpressionConverter<User, string>(user => $"{user.FirstName} {user.LastName}"));

        set.Bind(login)
            .For(x => x.Text)
            .To(vm => vm.UserContext.User.Email);

        set.Bind(ratingCounter)
            .For(v => v.Text)
            .To(vm => vm.UserContext.User.Rating);

        set.Bind(rewardsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.RewardsCollection);

        set.Bind(rate)
            .For(v => v.BindClick())
            .To(vm => vm.RateAppCommand);

        set.Bind(achievementsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.AchievementsCollection);

        set.Bind(friendsAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.FriendsCollection);

        set.Apply();
    }
}
