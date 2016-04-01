namespace DataCommander.Providers
{
    using System;

    public sealed class InfoMessage
    {
        public InfoMessage(
            DateTime creationTime,
            InfoMessageSeverity severity,
            string message)
        {
            this.CreationTime = creationTime;
            this.Severity = severity;
            this.Message = message;
        }

        public DateTime CreationTime { get; }

        public InfoMessageSeverity Severity { get; }

        public String Message { get; }
    }
}