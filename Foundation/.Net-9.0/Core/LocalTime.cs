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
            ClockAggregateRoot clock = ClockAggregateRepository.Singleton.Get();
            DateTime localTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount();
            return localTime;
        }
    }
}