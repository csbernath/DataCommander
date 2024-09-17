using Foundation.Core.ClockAggregate;

namespace DataCommander.Api.Connection;

public static class InfoMessageFactory
{
    public static InfoMessage Create(InfoMessageSeverity severity, string header, string message)
    {
        ClockAggregateRoot clock = ClockAggregateRepository.Singleton.Get();
        System.DateTime creationTime = clock.GetLocalTimeFromCurrentEnvironmentTickCount();
        return new InfoMessage(creationTime, severity, header, message);
    }
}