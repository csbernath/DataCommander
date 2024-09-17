using System;
using System.Threading;

namespace Foundation.Threading;

internal static class RetryEngine
{
    public static void Execute(Action action, int maxRetryCount, Func<Exception, RetryPolicyHandleExceptionResult> exceptionHandler, CancellationToken cancellationToken)
    {
        int tryCount = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ++tryCount;

            try
            {
                action();
                break;
            }
            catch (Exception exception)
            {
                if (tryCount == maxRetryCount)
                    throw;

                RetryPolicyHandleExceptionResult result = exceptionHandler(exception);

                if (!result.AnotherTryAllowed)
                    throw;

                if (result.WaitBeforeRetry != null)
                    cancellationToken.WaitHandle.WaitOne(result.WaitBeforeRetry.Value);
            }
        }
    }

    //public static async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> function, IRetryPolicy retryPolicy)
    //{
    //    TResult result;

    //    while (true)
    //    {
    //        retryPolicy.BeforeExecute();
    //        try
    //        {
    //            result = await function();
    //            break;
    //        }
    //        catch (Exception exception)
    //        {
    //            var throwException = await retryPolicy.HandleExceptionAsync(exception);
    //            if (throwException)
    //                throw;
    //        }
    //        finally
    //        {
    //            retryPolicy.AfterExecute();
    //        }
    //    }

    //    return result;
    //}
}