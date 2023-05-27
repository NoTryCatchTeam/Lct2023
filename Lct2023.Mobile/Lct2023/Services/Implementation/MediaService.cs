using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lct2023.Definitions.Exceptions;
using Lct2023.Definitions.Models;
using Xamarin.Essentials;

namespace Lct2023.Services.Implementation;

public class MediaService : IMediaService
{
    private readonly IDialogService _dialogService;
    private readonly IPermissionsService _permissionsService;

    public MediaService(IDialogService dialogService, IPermissionsService permissionsService)
    {
        _dialogService = dialogService;
        _permissionsService = permissionsService;
    }

    public async Task<FileResult> ChooseMediaAsync()
    {
        var buttonsDelegates = new List<(string Title, Func<Task<FileResult>> Task)>
        {
            ("Сделать фото", CapturePhotoAsync),
            ("Выбрать фото", PickPhotoAsync),
            ("Выбрать файл", PickFileAsync),
        };

        var mediaChoice = await _dialogService.ShowSheetAsync(new SheetConfig(
            buttonsDelegates.Select(x => x.Title),
            "Отмена"));

        if (buttonsDelegates.ElementAtOrDefault(mediaChoice) is var buttonDelegate &&
            buttonDelegate != default)
        {
            return await buttonDelegate.Task();
        }

        return null;
    }

    public async Task<FileResult> ChoosePhotoAsync()
    {
        var buttonsDelegates = new List<(string Title, Func<Task<FileResult>> Task)>
        {
            ("Сделать фото", CapturePhotoAsync),
            ("Выбрать фото", PickPhotoAsync),
        };

        var mediaChoice = await _dialogService.ShowSheetAsync(new SheetConfig(
            buttonsDelegates.Select(x => x.Title),
            "Отмена"));

        if (buttonsDelegates.ElementAtOrDefault(mediaChoice) is var buttonDelegate &&
            buttonDelegate != default)
        {
            return await buttonDelegate.Task();
        }

        return null;
    }

    private async Task<FileResult> PickFileAsync()
    {
        if (await _permissionsService.CheckAndRequestPermissionIfNeededAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
        {
            throw new HumanReadableException("Нет разрешения для чтения");
        }

        return await FilePicker.PickAsync();
    }

    private async Task<FileResult> CapturePhotoAsync()
    {
        if (await _permissionsService.CheckAndRequestPermissionIfNeededAsync<Permissions.Camera>() != PermissionStatus.Granted)
        {
            throw new HumanReadableException("Нет разрешения для использования камеры");
        }

        if (await _permissionsService.CheckAndRequestPermissionIfNeededAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
        {
            throw new HumanReadableException("Нет разрешения для записи");
        }

        return await MediaPicker.CapturePhotoAsync();
    }

    private async Task<FileResult> PickPhotoAsync()
    {
        if (await _permissionsService.CheckAndRequestPermissionIfNeededAsync<Permissions.Photos>() != PermissionStatus.Granted)
        {
            throw new HumanReadableException("Нет разрешения для доступа к галерее");
        }

        if (await _permissionsService.CheckAndRequestPermissionIfNeededAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
        {
            throw new HumanReadableException("Нет разрешения для чтения");
        }

        return await MediaPicker.PickPhotoAsync();
    }
}

public interface IMediaService
{
    Task<FileResult> ChooseMediaAsync();

    Task<FileResult> ChoosePhotoAsync();
}
