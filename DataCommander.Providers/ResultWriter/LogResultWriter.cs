using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Providers.Connection;
using DataCommander.Providers.QueryConfiguration;
using Foundation;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Data;
using Foundation.Data.DbQueryBuilding;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class LogResultWriter : IResultWriter
    {
        private class Result
        {
            public readonly ReadOnlyList<DbQueryResultField> Fields;

            public Result(ReadOnlyList<DbQueryResultField> fields) => Fields = fields;
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

        private string _fileName;
        private QueryConfiguration.Query _query;
        private ReadOnlyList<DbRequestParameter> _parameters;
        private string _commandText;
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
            var message = $"Executing command[{_commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
                message += "\r\n" + command.Parameters.ToLogString();

            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));

            _query = asyncDataAdapterCommand.Query;
            if (_query != null)
            {
                _fileName = asyncDataAdapterCommand.FileName;
                _parameters = asyncDataAdapterCommand.Parameters;
                _commandText = asyncDataAdapterCommand.CommandText;
                _results = new List<Result>();
            }
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
                var directory = _fileName != null ? Path.GetDirectoryName(_fileName) : Path.GetTempPath();
                var results = _query.Results.EmptyIfNull().Zip(_results, ToResult).ToReadOnlyList();
                var query = new DbRequest(directory, _query.Name, _query.Using, _query.Namespace, _commandText, 0, _parameters, results);

                var queryBuilder = new DbRequestBuilder(query);
                var csharpSourceCode = queryBuilder.Build();

                var path = Path.Combine(query.Directory, $"{query.Name}.generated.cs");
                File.WriteAllText(path, csharpSourceCode, Encoding.UTF8);

                _query = null;
                _parameters = null;
                _commandText = null;
                _results = null;
            }
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _writeTableBeginTimestamp = Stopwatch.GetTimestamp();

            Log.Trace($"SchemaTable of table[{_tableCount}], {schemaTable.TableName}:\r\n{schemaTable.ToStringTableString()}");

            if (_query != null)
            {
                var fields = schemaTable.Rows
                    .Cast<DataRow>()
                    .Select(FoundationDbColumnFactory.Create)
                    .Select(ToField)
                    .ToReadOnlyList();
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
                $"Reading {_rowCount} row(s) from command[{_commandCount - 1}] into table[{_tableCount - 1},{_query?.Results[_tableCount - 1]}] finished in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
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

        private static DbQueryResult ToResult(string result, Result sql)
        {
            Parser.ParseResult(result, out var name, out var fieldName);
            return new DbQueryResult(name, fieldName, sql.Fields);
        }

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

        private static DbQueryResultField ToField(FoundationDbColumn column)
        {
            return new DbQueryResultField(column.ColumnName, column.DataType, column.AllowDbNull == true);
        }

        #endregion
    }
}