using System.Threading;

namespace Foundation.Diagnostics;

internal sealed class InterlockedSequence(int value)
{
    private long _value = value;

    public long Next()
    {
        long next = Interlocked.Increment(ref _value);
        return next;
    }
}