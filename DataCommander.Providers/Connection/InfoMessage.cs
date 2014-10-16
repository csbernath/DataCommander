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

        public DateTime CreationTime
        {
            get
            {
                return this.creationTime;
            }
        }

        public InfoMessageSeverity Severity
        {
            get
            {
                return this.severity;
            }
        }

        public String Message
        {
            get
            {
                return this.message;
            }
        }
    }
}