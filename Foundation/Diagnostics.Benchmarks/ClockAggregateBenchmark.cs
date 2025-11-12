using System;
using BenchmarkDotNet.Attributes;
using Foundation.Core.ClockAggregate;

namespace Foundation.Diagnostics.Benchmarks;

public class ClockAggregateBenchmark
{
    [Benchmark]
    public DateTimeOffset GetUtcNow()
    {
        var utcNow = DateTimeOffset.UtcNow;
        return utcNow;
    }

    [Benchmark]
    public DateTimeOffset GetUtcFromCurrentEnvironmentTickCount64()
    {
        var clockAggregateRoot = ClockAggregateRepository.Singleton.Get();
        var utcNow = clockAggregateRoot.GetUtcFromCurrentEnvironmentTickCount64();
        return utcNow;
    }
}