using System.Threading.Tasks;

namespace Lct2023.Services;

public interface IDialogService
{
    Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
    
    void ShowToast(string text);
}