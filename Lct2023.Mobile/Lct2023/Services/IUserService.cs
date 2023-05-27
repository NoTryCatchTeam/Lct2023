using System.Threading;
using System.Threading.Tasks;
using Lct2023.Definitions.Dtos;

namespace Lct2023.Services;

public interface IUserService
{
    Task SignInViaSocialAsync(CancellationToken token);

    Task SignInAsync(string email, string password, CancellationToken token);

    Task SignUpAsync(CreateUserDto dto, CancellationToken token);

    Task SignOutAsync(CancellationToken token);
}
