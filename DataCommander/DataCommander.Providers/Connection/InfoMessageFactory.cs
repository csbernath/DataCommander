using Foundation.Core.ClockAggregate;

namespace DataCommander.Providers.Connection
{
    public static class InfoMessageFactory
    {
        public static InfoMessage Create(InfoMessageSeverity severity, string header, string message)
        {
            var clock = ClockAggregateRepository.Singleton.Get();
            var creationTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount();
            return new InfoMessage(creationTime, severity, header, message);
        }
    }
}