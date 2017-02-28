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

        void IResultWriter.Begin(IProvider provider)
        {
            this.beginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            this.beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
            var command = asyncDataAdapterCommand.Command;
            string message = $"Executing command[{this.commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            this.commandCount++;

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
            {
                message += "\r\n" + command.Parameters.ToLogString();
            }

            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var duration = Stopwatch.GetTimestamp() - this.beforeExecuteReaderTimestamp;
            string message = $"Command[{this.commandCount - 1}] started in {StopwatchTimeSpan.ToString(duration, 3)} seconds. Field count: {fieldCount}";
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            this.tableCount = 0;
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            var duration = Stopwatch.GetTimestamp() - this.beforeExecuteReaderTimestamp;
            var now = LocalTime.Default.Now;
            var affected = affectedRows >= 0
                ? $"{affectedRows} row(s) affected."
                : null;
            string message = $"Command[{this.commandCount - 1}] completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds. {affected}";
            this.addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.writeTableBeginTimestamp = Stopwatch.GetTimestamp();

            var now = LocalTime.Default.Now;
            this.addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, $"SchemaTable of table[{this.tableCount}]:\r\n{schemaTable.ToStringTableString()}"));

            this.tableCount++;
            this.rowCount = 0;
        }

        void IResultWriter.FirstRowReadBegin()
        {
            this.firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            var duration = Stopwatch.GetTimestamp() - this.firstRowReadBeginTimestamp;
            string message = $"First row read completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            this.rowCount += rowCount;
        }

        void IResultWriter.WriteTableEnd()
        {
            var duration = Stopwatch.GetTimestamp() - this.writeTableBeginTimestamp;
            string message =
                $"Reading {this.rowCount} row(s) from command[{this.commandCount - 1}] into table[{this.tableCount - 1}] finished in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            var duration = Stopwatch.GetTimestamp() - this.beginTimestamp;
            string message = $"Query completed {this.commandCount} command(s) in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        #endregion
    }
}