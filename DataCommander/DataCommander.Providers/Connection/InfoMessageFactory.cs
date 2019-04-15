using System;
using Foundation.Core;
using Foundation.Core.ClockAggregate;

namespace DataCommander.Providers.Connection
{
    public static class InfoMessageFactory
    {
        public static InfoMessage Create(InfoMessageSeverity severity, string header, string message)
        {
            var utcNow = ClockAggregateRepository.Get().GetUtcDateTimeFromEnvironmentTickCount(Environment.TickCount);
            var creationTime = utcNow.ToLocalTime();
            return new InfoMessage(creationTime, severity, header, message);
        }
    }
}