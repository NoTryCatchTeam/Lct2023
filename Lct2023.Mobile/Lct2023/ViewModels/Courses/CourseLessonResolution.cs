using System.IO;
using Xamarin.Essentials;

namespace Lct2023.ViewModels.Courses;

public class CourseLessonResolution
{
    public CourseLessonResolution(FileBase pickedMedia)
    {
        FullPath = pickedMedia.FullPath;
        FileName = pickedMedia.FileName;

        using var stream = File.OpenRead(FullPath);
        FileSize = stream.Length;
    }

    public string FullPath { get; }

    public string FileName { get; }

    public long FileSize { get; }
}