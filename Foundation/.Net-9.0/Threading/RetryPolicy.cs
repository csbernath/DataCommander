using System;
using System.Threading;

namespace Foundation.Threading;

public class RetryPolicy(int maxTryCount, TimeSpan beforeRetry, Func<Exception, bool> transient)
{
    public void Execute(Action action, CancellationToken cancellationToken) =>
        RetryEngine.Execute(action, maxTryCount, HandleException, cancellationToken);

    private RetryPolicyHandleExceptionResult HandleException(Exception exception)
    {
        bool anotherTryAllowed;
        TimeSpan? waitBeforeRetry;

        bool isTransient = transient(exception);
        if (isTransient)
        {
            anotherTryAllowed = true;
            waitBeforeRetry = beforeRetry;
        }
        else
        {
            anotherTryAllowed = false;
            waitBeforeRetry = null;
        }

        return new RetryPolicyHandleExceptionResult(anotherTryAllowed, waitBeforeRetry);
    }

    //async Task<bool> IRetryPolicy.HandleExceptionAsync(Exception exception)
    //{
    //    bool throwException;
    //    if (_tryCount < _maxTryCount)
    //    {
    //        var isTransient = _isTransient(exception);
    //        if (isTransient)
    //        {
    //            await Task.Delay(_waitTimeSpan, cancellationToken: _cancellationToken);
    //            _cancellationToken.ThrowIfCancellationRequested();
    //            throwException = false;
    //        }
    //        else
    //            throwException = true;
    //    }
    //    else
    //        throwException = true;

    //    return throwException;
    //}
}