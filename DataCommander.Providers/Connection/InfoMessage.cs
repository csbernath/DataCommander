namespace DataCommander.Providers
{
    using System;

    public sealed class InfoMessage
    {
        private readonly DateTime creationTime;
        private readonly InfoMessageSeverity severity;
        private readonly string message;

        public InfoMessage(
            DateTime creationTime,
            InfoMessageSeverity severity,
            string message)
        {
            this.creationTime = creationTime;
            this.severity = severity;
            this.message = message;
        }

        public DateTime CreationTime => this.creationTime;

        public InfoMessageSeverity Severity => this.severity;

        public String Message => this.message;
    }
}