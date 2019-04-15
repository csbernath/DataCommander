using System;
using System.Threading;

namespace Foundation.Core.Timers
{
    public static class ClockUpdater
    {
        private static Timer _timer;

        public static void Start()
        {
            Update();

            var period = TimeSpan.FromMinutes(5);
            _timer = new Timer(TimerCallback, null, period, period);
        }

        public static void Stop() => _timer.Dispose();

        private static void Update()
        {
            var clock = ClockFactory.CreateClock();
            ClockRepository.Save(clock);
        }

        private static void TimerCallback(object state) => Update();
    }
}