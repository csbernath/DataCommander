using System;
using Foundation.Core.ClockAggregate;

namespace Foundation.Core;

public sealed class LocalTime : IDateTimeProvider
{
    public static readonly LocalTime Default = new();

    private LocalTime()
    {
    }

    public DateTime Now
    {
        get
        {
            var clock = ClockAggregateRepository.Singleton.Get();
            var localTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount64();
            return localTime;
        }
    }
}