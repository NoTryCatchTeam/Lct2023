using System.IO;

namespace Lct2023.Services;

public interface IFileProvider
{
    Stream GetStream(string filePath);
}
