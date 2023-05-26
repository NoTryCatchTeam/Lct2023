using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Widget;
using AndroidX.AppCompat.App;
using Lct2023.Definitions.Models;
using Lct2023.Services;
using MvvmCross.Platforms.Android;

namespace Lct2023.Android.Services;

public class DialogService : Java.Lang.Object, IDialogService, IDialogInterfaceOnCancelListener
{
    private readonly IMvxAndroidCurrentTopActivity _currentTopActivity;
    private readonly IList<(AlertDialog dialog, TaskCompletionSource<bool> taskCompletionSource)> _alertDialogs;
    private readonly IList<(AlertDialog dialog, TaskCompletionSource<int> taskCompletionSource)> _alertSheetDialogs;

    public DialogService(IMvxAndroidCurrentTopActivity currentTopActivity)
    {
        _currentTopActivity = currentTopActivity;

        _alertDialogs = new List<(AlertDialog dialog, TaskCompletionSource<bool> taskCompletionSource)>();
        _alertSheetDialogs = new List<(AlertDialog dialog, TaskCompletionSource<int> taskCompletionSource)>();
    }

    public Task<bool> ShowDialogAsync(DialogConfig config)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        AlertDialog alert = null;

        var builder = new AlertDialog.Builder(_currentTopActivity.Activity)
            .SetTitle(config.Title)
            .SetMessage(config.Message)
            .SetCancelable(true)
            .SetOnCancelListener(this)
            .SetPositiveButton(config.PositiveButton, (_, _) =>
            {
                taskCompletionSource.SetResult(true);
                _alertDialogs.Remove((alert, taskCompletionSource));
            });

        if (!string.IsNullOrEmpty(config.NegativeButton))
        {
            builder.SetNegativeButton(config.NegativeButton, (_, _) =>
            {
                taskCompletionSource.SetResult(false);
                _alertDialogs.Remove((alert, taskCompletionSource));
            });
        }

        alert = builder.Create();

        _alertDialogs.Add((alert, taskCompletionSource));

        alert.Show();

        return taskCompletionSource.Task;
    }

    public Task<int> ShowSheetAsync(SheetConfig config)
    {
        var taskCompletionSource = new TaskCompletionSource<int>();

        var builder = new AlertDialog.Builder(_currentTopActivity.Activity)
            .SetCancelable(true)
            .SetOnCancelListener(this)
            .SetItems(config.Buttons.ToArray(), (_, e) => taskCompletionSource.SetResult(e.Which))
            .SetNegativeButton(config.CancelButton, (_, _) => taskCompletionSource.SetResult(-1));

        if (!string.IsNullOrEmpty(config.Title))
        {
            builder.SetTitle(config.Title);
        }

        var sheet = builder.Create();

        _alertSheetDialogs.Add((sheet, taskCompletionSource));

        sheet.Show();

        return taskCompletionSource.Task;
    }

    public Task CloseCurrentSheetAsync()
    {
        if (_alertSheetDialogs.LastOrDefault() is var sheetToCancel &&
            sheetToCancel == default)
        {
            return Task.CompletedTask;
        }

        sheetToCancel.dialog.Cancel();

        return Task.CompletedTask;
    }

    public void OnCancel(IDialogInterface dialog)
    {
        if (_alertDialogs.FirstOrDefault(x => ReferenceEquals(x.dialog, dialog)) is var canceledDialog &&
            canceledDialog != default)
        {
            canceledDialog.taskCompletionSource.TrySetResult(false);
            _alertDialogs.Remove((canceledDialog.dialog, canceledDialog.taskCompletionSource));

            return;
        }

        if (_alertSheetDialogs.FirstOrDefault(x => ReferenceEquals(x.dialog, dialog)) is var canceledSheetDialog &&
            canceledSheetDialog != default)
        {
            canceledSheetDialog.taskCompletionSource.TrySetResult(-1);
            _alertSheetDialogs.Remove((canceledSheetDialog.dialog, canceledSheetDialog.taskCompletionSource));
        }
    }

    public void ShowToast(string text)
        => Toast.MakeText(_currentTopActivity.Activity, text, ToastLength.Short)?.Show();
}
