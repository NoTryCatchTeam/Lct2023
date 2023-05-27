using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Lct2023.Services;

public interface IPermissionsService
{
    Task<PermissionStatus> CheckAndRequestPermissionIfNeededAsync<TPermission>()
        where TPermission : Permissions.BasePermission, new();
}