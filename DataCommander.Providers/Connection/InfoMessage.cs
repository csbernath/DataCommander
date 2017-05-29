namespace DataCommander.Providers.Connection
{
    using System;

    public sealed class InfoMessage
    {
        public InfoMessage(
            DateTime creationTime,
            InfoMessageSeverity severity,
            string message)
        {
            CreationTime = creationTime;
            Severity = severity;
            Message = message;
        }

        public DateTime CreationTime { get; }

        public InfoMessageSeverity Severity { get; }

        public string Message { get; }
    }
}