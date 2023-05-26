using System;
using Square.Picasso;
using Exception = Java.Lang.Exception;

namespace Lct2023.Android.Callbacks;

public class DefaultPicassoCallback : Java.Lang.Object, ICallback
{
    private readonly Action<Exception> _onError;
    private readonly Action _onSuccess;

    public DefaultPicassoCallback(Action<Exception> onError, Action onSuccess)
    {
        _onError = onError;
        _onSuccess = onSuccess;
    }

    public void OnError(Exception ex) =>
        _onError?.Invoke(ex);

    public void OnSuccess() =>
        _onSuccess?.Invoke();
}
