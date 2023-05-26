using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.Core.Content;
using Lct2023.Services;
using MimeTypes;

namespace Lct2023.Android.Services;

public class PlatformFileViewer : IPlatformFileViewer
{
    public Task OpenFileAsync(string filePath)
    {
        using var file = new Java.IO.File(filePath);

        var context = Application.Context;
        var uri = FileProvider.GetUriForFile(context, $"{context.PackageName}.fileprovider", file);
        var intent = new Intent(Intent.ActionView);
        intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
        intent.SetDataAndType(uri, MimeTypeMap.GetMimeType(uri.ToString()));

        context.StartActivity(intent);

        return Task.CompletedTask;
    }
}
