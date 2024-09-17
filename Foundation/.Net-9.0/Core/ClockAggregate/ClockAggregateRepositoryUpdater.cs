using System;
using System.Threading;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRepositoryUpdater
{
    private static Timer _timer;

    internal static void Start()
    {
        Update();

        var period = TimeSpan.FromMinutes(5);
        _timer = new Timer(TimerCallback, null, period, period);
    }

    public static void Stop() => _timer.Dispose();

    private static void Update()
    {
        var clock = ClockAggregateRootFactory.Now();
        ClockAggregateRepository.Singleton.Save(clock);
    }

    private static void TimerCallback(object state) => Update();
}