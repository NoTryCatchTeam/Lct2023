using System.Threading.Tasks;
using Lct2023.Definitions.Models;

namespace Lct2023.Services;

public interface IDialogService
{
    Task<bool> ShowDialogAsync(DialogConfig config);

    Task<int> ShowSheetAsync(SheetConfig config);

    void ShowToast(string text);
}
