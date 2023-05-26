using System.Threading.Tasks;

namespace Lct2023.Services;

public interface IPlatformFileViewer
{
    Task OpenFileAsync(string filePath);
}
