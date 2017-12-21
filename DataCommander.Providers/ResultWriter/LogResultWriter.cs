using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DataCommander.Providers.Connection;
using Foundation;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class LogResultWriter : IResultWriter
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Action<InfoMessage> _addInfoMessage;
        private int _commandCount;
        private int _tableCount;
        private int _rowCount;
        private long _beginTimestamp;
        private long _beforeExecuteReaderTimestamp;
        private long _writeTableBeginTimestamp;
        private long _firstRowReadBeginTimestamp;
        private readonly OrmBuilder _ormBuilder = new OrmBuilder(true);

        public LogResultWriter(Action<InfoMessage> addInfoMessage)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(addInfoMessage != null);
#endif
            _addInfoMessage = addInfoMessage;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
            _beginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            _beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
            var command = asyncDataAdapterCommand.Command;
            var message = $"Executing command[{_commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            ++_commandCount;

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
                message += "\r\n" + command.Parameters.ToLogString();

            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
            var message = $"Command[{_commandCount - 1}] started in {StopwatchTimeSpan.ToString(duration, 3)} seconds. Field count: {fieldCount}";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            _tableCount = 0;
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
            var now = LocalTime.Default.Now;
            var affected = affectedRows >= 0
                ? $"{affectedRows} row(s) affected."
                : null;
            var message = $"Command[{_commandCount - 1}] completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds. {affected}";
            _addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _writeTableBeginTimestamp = Stopwatch.GetTimestamp();

            var objectId = _tableCount + 1;
            var objectTypeName = $"Object{objectId}";
            var columns = schemaTable.Rows.Cast<DataRow>().Select(i => new DbColumn(i)).ToList();
            _ormBuilder.Add(objectTypeName, columns);
            Log.Trace($"SchemaTable of table[{_tableCount}]:\r\n{schemaTable.ToStringTableString()}");
            ++_tableCount;
            _rowCount = 0;
        }

        void IResultWriter.FirstRowReadBegin()
        {
            _firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            var duration = Stopwatch.GetTimestamp() - _firstRowReadBeginTimestamp;
            var message = $"First row read completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            _rowCount += rowCount;
        }

        void IResultWriter.WriteTableEnd()
        {
            var duration = Stopwatch.GetTimestamp() - _writeTableBeginTimestamp;
            var message =
                $"Reading {_rowCount} row(s) from command[{_commandCount - 1}] into table[{_tableCount - 1}] finished in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            var duration = Stopwatch.GetTimestamp() - _beginTimestamp;
            var message = $"Query completed {_commandCount} command(s) in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));

            Log.Trace($"\r\n{_ormBuilder}");
        }

        #endregion
    }
}