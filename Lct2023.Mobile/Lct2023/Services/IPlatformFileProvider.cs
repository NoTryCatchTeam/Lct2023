using System.IO;

namespace Lct2023.Services;

public interface IPlatformFileProvider
{
    Stream GetStream(string filePath);
}
