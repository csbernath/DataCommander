using System;
using System.Threading;

namespace Foundation.Diagnostics;

public sealed class Int64PerformanceCounter
{
    private readonly string _name;
    private readonly Func<long, string> _toString;
    private long _count;
    private long _sum;
    private long _min = long.MaxValue;
    private long _max = long.MinValue;

    public Int64PerformanceCounter(string name, Func<long, string> toString)
    {
        ArgumentNullException.ThrowIfNull(toString);

        _name = name;
        _toString = toString;
    }

    public void Increment(long item)
    {
        Interlocked.Increment(ref _count);
        Interlocked.Add(ref _sum, item);

        while (true)
        {
            var min = _min;
            if (item < min)
            {
                var originalMin = Interlocked.CompareExchange(ref _min, item, min);
                if (originalMin == min)
                    break;
                else
                    Thread.SpinWait(1);
            }
            else
                break;
        }

        while (true)
        {
            var max = _max;
            if (item > max)
            {
                var originalMax = Interlocked.CompareExchange(ref _max, item, max);
                if (originalMax == max)
                    break;
                else
                    Thread.SpinWait(1);
            }
            else
                break;
        }
    }

    public long Count => _count;
    public long Sum => _sum;
    public long Min => _min;
    public long Max => _max;

    public string ToLogString() => $@"Int64PerformanceCounter '{_name}'
count: {_count}
min: {_toString(_min)}
avg: {_toString((long)((double)Sum / Count))}
max: {_toString(_max)}
sum: {_toString(_sum)}";
}