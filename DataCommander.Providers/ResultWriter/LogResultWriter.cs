using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Providers.Connection;
using Foundation;
using Foundation.Assertions;
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
        private string _commandText;
        private string _namespace;
        private string _ormQuery;
        private Queue<string> _ormRecord;
        private readonly List<OrmResult> _ormResults = new List<OrmResult>();

        public LogResultWriter(Action<InfoMessage> addInfoMessage)
        {
            Assert.IsNotNull(addInfoMessage);
            _addInfoMessage = addInfoMessage;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider) => _beginTimestamp = Stopwatch.GetTimestamp();

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            _beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
            var command = asyncDataAdapterCommand.Command;
            var message = $"Executing command[{_commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            ++_commandCount;
            _commandText = command.CommandText;
            GetOrm(_commandText, out _namespace, out _ormQuery, out _ormRecord);

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
                message += "\r\n" + command.Parameters.ToLogString();

            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
            var message = $"Command[{_commandCount - 1}] started in {StopwatchTimeSpan.ToString(duration, 3)} seconds. Field count: {fieldCount}";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
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
            _addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, null, message));
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _writeTableBeginTimestamp = Stopwatch.GetTimestamp();

            Log.Trace($"SchemaTable of table[{_tableCount}]:\r\n{schemaTable.ToStringTableString()}");

            string recordClassName;
            if (_ormRecord.Count > 0)
                recordClassName = _ormRecord.Dequeue();
            else
            {
                var recordId = _tableCount + 1;
                recordClassName = $"Record{recordId}";
            }

            var columns = schemaTable.Rows
                .Cast<DataRow>()
                .Select(FoundationDbColumnFactory.Create)
                .Select(ToOrmColumn)
                .ToReadOnlyCollection();
            var ormResult = new OrmResult(recordClassName, columns);
            _ormResults.Add(ormResult);

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
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
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
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            var duration = Stopwatch.GetTimestamp() - _beginTimestamp;
            var message = $"Query completed {_commandCount} command(s) in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));

            var ormBuilder = new OrmBuilder(_commandText, _namespace, _ormQuery,_ormResults.AsReadOnly());
            var orm = ormBuilder.ToString(false);
            Log.Trace($"\r\n{orm}");

            var s = LocalTime.Default.Now.ToString("yyyy.MM.dd HHmmss.fff");
            var path = Path.Combine(Path.GetTempPath(), $"DataCommander.Orm.{s}.cs");
            File.WriteAllText(path, orm, Encoding.UTF8);
        }

        #endregion

        #region Private Methods

        private static void GetOrm(string commandText, out string @namespace, out string query, out Queue<string> record)
        {
            @namespace = null;
            query = null;
            record = new Queue<string>();
            using (var reader = new StringReader(commandText))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (@namespace == null && line.StartsWith("--OrmNamespace:"))
                        @namespace = line.Substring(15);
                    if (query == null && line.StartsWith("--OrmQuery:"))
                        query = line.Substring(11);
                    else if (line.StartsWith("--OrmRecord:"))
                    {
                        var typeName = line.Substring(12);
                        record.Enqueue(typeName);
                    }
                }
            }
        }

        private static OrmColumn ToOrmColumn(FoundationDbColumn column) => new OrmColumn(column.ColumnName, column.DataType, column.AllowDbNull == true);

        #endregion
    }
}