using System;
using Foundation.Assertions;

namespace Foundation.Threading;

public class RetryPolicyHandleExceptionResult
{
    public readonly bool AnotherTryAllowed;
    public readonly TimeSpan? WaitBeforeRetry;

    public RetryPolicyHandleExceptionResult(bool anotherTryAllowed, TimeSpan? waitBeforeRetry)
    {
        Assert.IsTrue((anotherTryAllowed && waitBeforeRetry == null) || (!anotherTryAllowed && waitBeforeRetry != null));

        AnotherTryAllowed = anotherTryAllowed;
        WaitBeforeRetry = waitBeforeRetry;
    }
}