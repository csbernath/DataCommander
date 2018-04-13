using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Providers.Connection;
using Foundation;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.DbQueryBuilding;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;
using Newtonsoft.Json;
using Parameter = DataCommander.Providers.ResultWriter.QueryConfiguration.Parameter;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class LogResultWriter : IResultWriter
    {
        private class Result
        {
            public readonly ReadOnlyCollection<DbQueryResultField> Fields;

            public Result(ReadOnlyCollection<DbQueryResultField> fields)
            {
                Fields = fields;
            }
        }

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
        private QueryConfiguration.Query _query;
        private List<Result> _results;

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
            ++_commandCount;

            var command = asyncDataAdapterCommand.Command;
            var commandText = command.CommandText;
            if (commandText.StartsWith("/* Query"))
            {
                var index = command.CommandText.IndexOf("*/");
                _commandText = commandText.Substring(index + 4);
                var json = commandText.Substring(10, index - 10);
                _query = JsonConvert.DeserializeObject<QueryConfiguration.Query>(json);
                _results = new List<Result>();
                command.CommandText = GetParameterizedCommandText(_query.Parameters, _commandText);
            }

            var message = $"Executing command[{_commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
                message += "\r\n" + command.Parameters.ToLogString();

            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
        }

        private static string GetParameterizedCommandText(ReadOnlyCollection<Parameter> parameters, string commandText)
        {
            var stringBuilder = new StringBuilder();
            foreach (var parameter in parameters)
                stringBuilder.Append($"declare @{parameter.Name} {parameter.DataType}{parameter.Value}\r\n");
            stringBuilder.Append(commandText);
            return stringBuilder.ToString();
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

            if (_query != null)
            {
                var parameters = _query.Parameters.Select(ToParameter).ToReadOnlyCollection();
                var results = _query.Results.Zip(_results, ToResult).ToReadOnlyCollection();
                var query = new DbQuery(_query.Using, _query.Namespace, _query.Name, _commandText, 0, parameters, results);

                var queryBuilder = new DbQueryBuilder(query);
                var csharpSourceCode = queryBuilder.Build();
                Log.Trace($"\r\n{csharpSourceCode}");

                var timestamp = LocalTime.Default.Now.ToString("yyyy.MM.dd HHmmss.fff");
                var path = Path.Combine(Path.GetTempPath(), $"DataCommander.Orm.{timestamp}.cs");
                File.WriteAllText(path, csharpSourceCode, Encoding.UTF8);

                _query = null;
                _results = null;
            }
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _writeTableBeginTimestamp = Stopwatch.GetTimestamp();

            Log.Trace($"SchemaTable of table[{_tableCount}]:\r\n{schemaTable.ToStringTableString()}");

            if (_query != null)
            {
                var fields = schemaTable.Rows
                    .Cast<DataRow>()
                    .Select(FoundationDbColumnFactory.Create)
                    .Select(ToField)
                    .ToReadOnlyCollection();
                var result = new Result(fields);
                _results.Add(result);
            }

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
        }

        private DbQueryParameter ToParameter(Parameter source) =>
            new DbQueryParameter(source.Name, source.DataType, source.SqlDbType, source.CSharpDataType, source.IsNullable, source.CSharpValue);

        private DbQueryResult ToResult(QueryConfiguration.Result config, Result sql) => new DbQueryResult(config.Name, config.FieldName, sql.Fields);

        #endregion

        #region Private Methods

        //private static Foundation.Data.SqlClient.Orm.Query GetQuery(string commandText, out string @namespace, out Foundation.Data.SqlClient.Orm.Query query, out Queue<string> record)
        //{
        //    @namespace = null;
        //    string queryName = null;
        //    var parameters = new List<OrmParameter>();
        //    record = new Queue<string>();
        //    using (var reader = new StringReader(commandText))
        //    {
        //        while (reader.Peek() >= 0)
        //        {
        //            var line = reader.ReadLine();
        //            if (@namespace == null && line.StartsWith("--namespace:"))
        //                @namespace = line.Substring(12);
        //            if (queryName == null && line.StartsWith("--query:"))
        //                queryName = line.Substring(8);
        //            else if (line.StartsWith("--parameter:"))
        //            {
        //                var parameter = GetOrmParameter(line.Substring(12));
        //                parameters.Add(parameter);
        //            }
        //            else if (line.StartsWith("--record:"))
        //            {
        //                var typeName = line.Substring(9);
        //                record.Enqueue(typeName);
        //            }
        //        }
        //    }

        //    query = new Query(queryName, parameters.AsReadOnly());
        //}

        private static DbQueryResultField ToField(FoundationDbColumn column) => new DbQueryResultField(column.ColumnName, column.DataType, column.AllowDbNull == true);

        #endregion
    }
}