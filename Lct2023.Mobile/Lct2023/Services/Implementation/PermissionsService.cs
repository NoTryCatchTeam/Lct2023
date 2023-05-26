using System.Threading.Tasks;
using Lct2023.Definitions.Models;
using Xamarin.Essentials;

namespace Lct2023.Services.Implementation;

public class PermissionsService : IPermissionsService
{
    private readonly IDialogService _dialogService;

    public PermissionsService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<PermissionStatus> CheckAndRequestPermissionIfNeededAsync<TPermission>()
        where TPermission : Permissions.BasePermission, new()
    {
        var status = await Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
        {
            return status;
        }

        if (Permissions.ShouldShowRationale<TPermission>() &&
            !await _dialogService.ShowDialogAsync(new DialogConfig(
                "Недостаточно разрешений",
                "Приложению не хватает разрешений для полной работы. Нажмите \"Запросить\", чтобы запросить разрешения еще раз",
                "Запросить",
                "Отмена")))
        {
            return status;
        }

        return await Permissions.RequestAsync<TPermission>();
    }
}