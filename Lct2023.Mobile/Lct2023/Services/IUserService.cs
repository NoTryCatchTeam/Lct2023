using System.Threading;
using System.Threading.Tasks;

namespace Lct2023.Services;

public interface IUserService
{
    Task SignInViaSocialAsync(CancellationToken token);

    Task SignInAsync(string login, string password, CancellationToken token);

    Task SignOutAsync(CancellationToken token);
}
