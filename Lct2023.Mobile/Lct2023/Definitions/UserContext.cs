using System.Threading.Tasks;
using Lct2023.Definitions.Internal;
using Lct2023.Services;
using MvvmCross.ViewModels;

namespace Lct2023.Definitions;

public class UserContext : MvxNotifyPropertyChanged, IUserContext
{
    private readonly ISecureStorageService _secureStorageService;

    private const string USER_DATA_KEY = "UserData";

    private User _user;

    public UserContext(ISecureStorageService secureStorageService)
    {
        _secureStorageService = secureStorageService;
    }

    public bool IsAuthenticated => User != null;

    public User User
    {
        get => _user;
        private set
        {
            if (!SetProperty(ref _user, value))
            {
                return;
            }

            RaisePropertyChanged(nameof(IsAuthenticated));
        }
    }

    public async Task StoreAsync(User user)
    {
        if (!string.IsNullOrEmpty(await _secureStorageService.GetValueAsync(USER_DATA_KEY)))
        {
            _secureStorageService.Remove(USER_DATA_KEY);
        }

        await _secureStorageService.SetValueAsync(USER_DATA_KEY, user);

        User = user;
    }

    public void Reset()
    {
        _secureStorageService.RemoveAll();

        User = null;
    }

    public async Task RestoreAsync()
    {
        User = await _secureStorageService.GetValueAsync<User>(USER_DATA_KEY);
    }
}

public interface IUserContext
{
    bool IsAuthenticated { get; }

    User User { get; }

    Task StoreAsync(User user);

    void Reset();

    Task RestoreAsync();
}
