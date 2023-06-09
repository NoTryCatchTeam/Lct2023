using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lct2023.Helpers;

public static class TaskExtensions
{
    public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout, Func<TResult> onTimeout = null)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();

        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));

        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();

            return await task;
        }

        return onTimeout != null ?
            onTimeout.Invoke() :
            throw new TimeoutException($"The operation has timed out after {timeout:mm\\:ss}");
    }

    public static async Task TimeoutAfter(this Task task, TimeSpan timeout, Action onTimeout = null)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();

        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));

        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();

            await task;

            return;
        }

        if (onTimeout != null)
        {
            onTimeout();
        }
        else
        {
            throw new TimeoutException($"The operation has timed out after {timeout:mm\\:ss}");
        }
    }
}
