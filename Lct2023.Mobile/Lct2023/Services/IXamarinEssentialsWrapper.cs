using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Lct2023.Services;

public interface IXamarinEssentialsWrapper
{
    Task RunOnUiAsync(Func<Task> factory);
    
    Task<TResult> RunOnUiAsync<TResult>(Func<Task<TResult>> factory);
    
    void RunOnUi(Action action);

    Task<Location> GetCurrentLocationAsync(CancellationToken token);
}