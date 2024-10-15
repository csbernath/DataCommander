using Foundation.Core.ClockAggregate;

namespace DataCommander.Api.Connection;

public static class InfoMessageFactory
{
    public static InfoMessage Create(InfoMessageSeverity severity, string header, string message)
    {
        var clock = ClockAggregateRepository.Singleton.Get();
        var creationTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount64();
        return new InfoMessage(creationTime, severity, header, message);
    }
}