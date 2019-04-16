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
                var clock = ClockAggregateRepository.Get();
                var univeralTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount();
                return univeralTime;
            }
        }
    }
}