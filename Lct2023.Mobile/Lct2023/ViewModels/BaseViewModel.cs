using System;
using System.Threading;
using System.Threading.Tasks;
using Lct2023.Services;
using Lct2023.ViewModels.AppRate;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using PropertyChanged;

namespace Lct2023.ViewModels;

public abstract class BaseViewModel : MvxNavigationViewModel
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    protected BaseViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        NavigateBackCommand = new MvxAsyncCommand(NavigateBackAction);
        RateAppCommand = new MvxAsyncCommand(() => NavigationService.Navigate<AppRateViewModel>());

        _cancellationTokenSource = new CancellationTokenSource();
    }

    [MvxInject]
    public IUserContext UserContext { get; set; }

    public IMvxAsyncCommand NavigateBackCommand { get; }

    public IMvxAsyncCommand RateAppCommand { get; }

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    protected async Task RunSafeTaskAsync(Func<Task> task, Func<Exception, Task> onException = null)
    {
        try
        {
            await task();
        }
        catch (Exception ex)
        {
            Log.LogError(ex, ex.Message);

            if (onException != null)
            {
                await onException.Invoke(ex);
            }
        }
    }

    protected virtual Task NavigateBackAction() =>
        NavigationService.Close(this);

    public override void ViewDestroy(bool viewFinishing = true)
    {
        base.ViewDestroy(viewFinishing);

        _cancellationTokenSource?.Cancel();
    }
}

public abstract class BaseViewModel<TParameter> : BaseViewModel, IMvxViewModel<TParameter>
    where TParameter : class
{
    protected BaseViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }

    public TParameter NavigationParameter { get; private set; }

    public virtual void Prepare(TParameter parameter)
    {
        NavigationParameter = parameter;
    }
}

public abstract class BaseViewModelResult<TResult> : BaseViewModel, IMvxViewModelResult<TResult>
    where TResult : class
{
    protected BaseViewModelResult(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }

    public TaskCompletionSource<object> CloseCompletionSource { get; set; }

    public override void ViewDestroy(bool viewFinishing = true)
    {
        if (viewFinishing && CloseCompletionSource != null &&
            !CloseCompletionSource.Task.IsCompleted &&
            !CloseCompletionSource.Task.IsFaulted)
        {
            CloseCompletionSource.TrySetCanceled();
        }

        base.ViewDestroy(viewFinishing);
    }
}

public abstract class BaseViewModel<TParameter, TResult> : BaseViewModelResult<TResult>, IMvxViewModel<TParameter, TResult>
    where TParameter : class
    where TResult : class
{
    protected BaseViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
    }

    public TParameter NavigationParameter { get; private set; }

    protected abstract override Task NavigateBackAction();

    public virtual void Prepare(TParameter parameter)
    {
        NavigationParameter = parameter;
    }
}
