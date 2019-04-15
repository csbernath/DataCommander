using Foundation.Core.ClockAggregate;
using System;

namespace Foundation.Core
{
    public sealed class UniversalTime : IDateTimeProvider
    {
        public static readonly UniversalTime Default = new UniversalTime();

        private UniversalTime()
        {
        }

        public DateTime Now
        {
            get
            {
                var utcNow = ClockAggregateRepository.Get().GetUtcDateTimeFromEnvironmentTickCount(Environment.TickCount);
                return utcNow;
            }
        }
    }
}