using System;
using Foundation.Core.ClockAggregate;

namespace Foundation.Core
{
    public sealed class LocalTime : IDateTimeProvider
    {
        public static readonly LocalTime Default = new LocalTime();

        private LocalTime()
        {
        }

        public DateTime Now
        {
            get
            {
                var utcNow = ClockAggregateRepository.Get().GetUtcDateTimeFromEnvironmentTickCount(Environment.TickCount);
                var now = utcNow.ToLocalTime();
                return now;
            }
        }
    }
}