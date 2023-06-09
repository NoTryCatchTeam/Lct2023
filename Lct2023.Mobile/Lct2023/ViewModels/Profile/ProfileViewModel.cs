using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Profile;

public class ProfileViewModel : BaseViewModel
{
    public ProfileViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        RewardsCollection = GetRewards();
        AchievementsCollection = GetAchievements();
        FriendsCollection = GetFriends();
    }

    public IEnumerable<ProfileRewardItem> RewardsCollection { get; }

    public IEnumerable<AchievementItem> AchievementsCollection { get; }

    public IEnumerable<FriendItem> FriendsCollection { get; }

    private IEnumerable<ProfileRewardItem> GetRewards() =>
        new[]
        {
            new ProfileRewardItem
            {
                Points = 5,
                IsCollected = true,
            },
            new ProfileRewardItem
            {
                Points = 10,
                IsAvailable = true,
            },
            new ProfileRewardItem
            {
                Points = 10,
            },
            new ProfileRewardItem
            {
                Points = 15,
            },
            new ProfileRewardItem
            {
                Points = 15,
            },
            new ProfileRewardItem
            {
                Points = 20,
            },
            new ProfileRewardItem
            {
                Points = 20,
            },
        };

    private IEnumerable<AchievementItem> GetAchievements() =>
        new[] { 0, 1, 2, 3, 4 }.Select(x => new AchievementItem { Number = x }).ToList();

    private IEnumerable<FriendItem> GetFriends() =>
        new[]
        {
            new FriendItem
            {
                Name = "Сергей Колчин",
                Rating = 1065,
            },
            new FriendItem
            {
                Name = "Дмитрий Орденов",
                Rating = 600,
            },
            new FriendItem
            {
                Name = "Андрей Макаров",
                Rating = 338,
            },
            new FriendItem
            {
                Name = "Сергей Владимиров",
                Rating = 201,
            },
            new FriendItem
            {
                Name = "Елизавета Новикова",
                Rating = 100,
            },
        };
}
