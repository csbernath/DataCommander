using System;

namespace Foundation.Core.ClockAggregate;

public class FoundationTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        var now = ClockAggregateRootFactory.Now();
        return now.GetUniversalTimeOffset();
    }
}