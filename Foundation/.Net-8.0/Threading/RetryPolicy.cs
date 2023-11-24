using System;
using System.Threading;

namespace Foundation.Threading;

public class RetryPolicy
{
    private readonly int _maxTryCount;
    private readonly TimeSpan _waitBeforeRetry;
    private readonly Func<Exception, bool> _isTransient;

    public RetryPolicy(int maxTryCount, TimeSpan waitBeforeRetry, Func<Exception, bool> isTransient)
    {
        _maxTryCount = maxTryCount;
        _waitBeforeRetry = waitBeforeRetry;
        _isTransient = isTransient;
    }

    public void Execute(Action action, CancellationToken cancellationToken) =>
        RetryEngine.Execute(action, _maxTryCount, HandleException, cancellationToken);

    private RetryPolicyHandleExceptionResult HandleException(Exception exception)
    {
        bool anotherTryAllowed;
        TimeSpan? waitBeforeRetry;

        var isTransient = _isTransient(exception);
        if (isTransient)
        {
            anotherTryAllowed = true;
            waitBeforeRetry = _waitBeforeRetry;
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