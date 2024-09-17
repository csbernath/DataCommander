using Foundation.Core.ClockAggregate;
using System;

namespace Foundation.Core;

public sealed class UniversalTime : IDateTimeProvider
{
    public static readonly UniversalTime Default = new();

    private UniversalTime()
    {
    }

    public DateTime Now
    {
        get
        {
            ClockAggregateRoot clock = ClockAggregateRepository.Singleton.Get();
            DateTime universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount();
            return universalTime;
        }
    }
}