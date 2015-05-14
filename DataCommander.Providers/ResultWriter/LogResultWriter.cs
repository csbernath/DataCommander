namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    internal sealed class LogResultWriter : IResultWriter
    {
        private readonly Action<InfoMessage> addInfoMessage;
        private int commandCount;
        private int tableCount;
        private int rowCount;
        private long beginTimestamp;
        private long beforeExecuteReaderTimestamp;
        private long writeTableBeginTimestamp;
        private long firstRowReadBeginTimestamp;

        public LogResultWriter(Action<InfoMessage> addInfoMessage)
        {
            Contract.Requires<ArgumentNullException>(addInfoMessage != null);
            this.addInfoMessage = addInfoMessage;
        }

        #region IResultWriter Members

        void IResultWriter.Begin()
        {
            this.beginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.BeforeExecuteReader(IProvider provider, IDbCommand command)
        {
            this.beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
            string message = string.Format("Executing command[{0}]\r\n{1}",
                this.commandCount,
                command.CommandText);

            this.commandCount++;

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
            {
                message += "\r\n" + command.Parameters.ToLogString();
            }

            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.AfterExecuteReader()
        {
            long duration = Stopwatch.GetTimestamp() - this.beforeExecuteReaderTimestamp;
            string message = string.Format("Command[{0}] started in {1} seconds.", this.commandCount - 1, StopwatchTimeSpan.ToString(duration, 3));
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            this.tableCount = 0;
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            long duration = Stopwatch.GetTimestamp() - this.beforeExecuteReaderTimestamp;
            DateTime now = LocalTime.Default.Now;
            string message = string.Format(
                "Command[{0}] completed in {1} seconds.",
                this.commandCount - 1,
                StopwatchTimeSpan.ToString(duration, 3));
            this.addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, message));

            if (affectedRows >= 0)
            {
                message = string.Format("{0} row(s) affected.", affectedRows);
                this.addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, message));
            }
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.writeTableBeginTimestamp = Stopwatch.GetTimestamp();
            this.tableCount++;
            this.rowCount = 0;
        }

        void IResultWriter.FirstRowReadBegin()
        {
            this.firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            long duration = Stopwatch.GetTimestamp() - this.firstRowReadBeginTimestamp;
            string message = string.Format("First row read completed in {0} seconds.", StopwatchTimeSpan.ToString(duration, 3));
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            this.rowCount += rowCount;
        }

        void IResultWriter.WriteTableEnd()
        {
            long duration = Stopwatch.GetTimestamp() - this.writeTableBeginTimestamp;
            string message = string.Format(
                "Reading {0} row(s) from command[{1}] into table[{2}] finished in {3} seconds.",
                this.rowCount,
                this.commandCount - 1,
                this.tableCount - 1,
                StopwatchTimeSpan.ToString(duration, 3));
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            long duration = Stopwatch.GetTimestamp() - this.beginTimestamp;
            string message = string.Format(
                "Query completed {0} command(s) in {1} seconds.",
                this.commandCount,
                StopwatchTimeSpan.ToString(duration, 3));
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        #endregion
    }
}