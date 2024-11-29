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
            var clock = ClockAggregateRepository.Singleton.Get();
            var universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount64();
            return universalTime.DateTime;
        }
    }
}