using System;
using System.Threading.Tasks;

namespace Lct2023.Services;

public interface IXamarinEssentialsWrapper
{
    Task RunOnUiAsync(Func<Task> factory);
    
    Task<TResult> RunOnUiAsync<TResult>(Func<Task<TResult>> factory);
    
    void RunOnUi(Action action);
}