using System;

namespace DataCommander.Providers2.Connection;

public sealed class InfoMessage
{
    public readonly DateTime CreationTime;
    public readonly InfoMessageSeverity Severity;
    public readonly string Header;
    public readonly string Message;

    public InfoMessage(DateTime creationTime, InfoMessageSeverity severity, string header, string message)
    {
        CreationTime = creationTime;
        Severity = severity;
        Header = header;
        Message = message;
    }
}