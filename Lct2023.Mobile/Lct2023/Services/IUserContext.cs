using System.ComponentModel;
using System.Threading.Tasks;
using Lct2023.Definitions.Internals;

namespace Lct2023.Services;

public interface IUserContext : INotifyPropertyChanged
{
    bool IsAuthenticated { get; }

    User User { get; }

    Task StoreAsync(User user);

    void Reset();

    Task RestoreAsync();
}
