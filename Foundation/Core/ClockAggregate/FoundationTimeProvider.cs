using System;

namespace Foundation.Core.ClockAggregate;

public class FoundationTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        var clockAggregateRoot = ClockAggregateRepository.Singleton.Get();
        var utcNow = clockAggregateRoot.GetUtcFromCurrentEnvironmentTickCount64();
        return utcNow;
    }
}