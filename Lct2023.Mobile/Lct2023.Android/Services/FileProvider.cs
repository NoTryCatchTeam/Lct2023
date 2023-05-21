using System.IO;
using Android.App;
using Lct2023.Services;

namespace Lct2023.Android.Services;

public class FileProvider : IFileProvider
{
    public Stream GetStream(string filePath) =>
        Application.Context.Assets!.Open(filePath);
}
