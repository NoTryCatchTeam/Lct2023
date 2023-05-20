using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Lct2023.Services.Implementation;

public class XamarinEssentialsWrapper : IXamarinEssentialsWrapper
{
    public Task RunOnUiAsync(Func<Task> factory) => MainThread.InvokeOnMainThreadAsync(factory);

    public Task<TResult> RunOnUiAsync<TResult>(Func<Task<TResult>> factory) => MainThread.InvokeOnMainThreadAsync(factory);

    public void RunOnUi(Action action) => MainThread.BeginInvokeOnMainThread(action);
}