using System;

namespace DataCommander.Api.Connection;

public sealed class InfoMessage(DateTime creationTime, InfoMessageSeverity severity, string? header, string message)
{
    public readonly DateTime CreationTime = creationTime;
    public readonly InfoMessageSeverity Severity = severity;
    public readonly string? Header = header;
    public readonly string Message = message;
}