using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.QueryConfiguration;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient.DbQueryBuilding;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Application.ResultWriter;

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

    private string _fileName;
    private Api.QueryConfiguration.Query _query;
    private ReadOnlyCollection<DbRequestParameter> _parameters;
    private string _commandText;
    private List<Result> _results;

    public LogResultWriter(Action<InfoMessage> addInfoMessage)
    {
        ArgumentNullException.ThrowIfNull(addInfoMessage);
        _addInfoMessage = addInfoMessage;
    }

    #region IResultWriter Members

    void IResultWriter.Begin(IProvider provider) => _beginTimestamp = Stopwatch.GetTimestamp();

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
    {
        _beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
        ++_commandCount;

        var command = asyncDataAdapterCommand.Command;
        const long duration = 0;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        var message = $"Before executing reader at line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";
        var parameters = command.Parameters;
        if (!parameters.IsNullOrEmpty())
            message += "\r\n" + command.Parameters.ToLogString();

        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        _query = asyncDataAdapterCommand.Query;
        if (_query != null)
        {
            _fileName = asyncDataAdapterCommand.FileName;
            _parameters = asyncDataAdapterCommand.Parameters;
            _commandText = asyncDataAdapterCommand.CommandText;
            _results = new List<Result>();
        }
    }

    void IResultWriter.AfterExecuteReader()
    {
        var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount - 1}]";
        var message = "Executing reader...";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        _tableCount = 0;
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
        var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Reader closed.");
        if (affectedRows >= 0)
            stringBuilder.Append($" {SingularOrPlural(affectedRows, "row", "rows")} affected.");
        var message = stringBuilder.ToString();
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        if (_query != null)
        {
            var directory = _fileName != null ? Path.GetDirectoryName(_fileName) : Path.GetTempPath();
            var results = _query.Results.EmptyIfNull().Zip(_results, ToResult).ToReadOnlyCollection();
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
        ++_tableCount;
        _rowCount = 0;        
        
        var duration = _writeTableBeginTimestamp - _beforeExecuteReaderTimestamp;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        var message =
            $"Result[{_tableCount - 1}] has {SingularOrPlural(schemaTable.Rows.Count, "column", "columns")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        Log.Trace($"SchemaTable of table[{_tableCount - 1}], {schemaTable.TableName}:\r\n{schemaTable.ToStringTableString()}");

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
    }

    void IResultWriter.FirstRowReadBegin() => _firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
        var duration = Stopwatch.GetTimestamp() - _firstRowReadBeginTimestamp;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        var message = $"Result[{_tableCount - 1}] first row read completed.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount) => _rowCount += rowCount;

    void IResultWriter.WriteTableEnd()
    {
        var duration = Stopwatch.GetTimestamp() - _writeTableBeginTimestamp;
        var header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount - 1}]";
        var message =
            $"Result[{_tableCount - 1}] has {SingularOrPlural(_rowCount, "row", "rows")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
    }

    private static string SingularOrPlural(int count, string singular, string plural)
    {
        return count == 1
            ? $"{count} {singular}"
            : $"{count} {plural}";
    }

    void IResultWriter.End()
    {
        var duration = Stopwatch.GetTimestamp() - _beginTimestamp;
        var header = StopwatchTimeSpan.ToString(duration, 3);
        var message = $"Query completed {SingularOrPlural(_commandCount, "command", "commands")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    #endregion

    #region Private Methods

    private static DbQueryResult ToResult(string result, Result sql)
    {
        Parser.ParseResult(result, out var name, out var fieldName);
        return new DbQueryResult(name, fieldName, sql.Fields);
    }

    private static DbQueryResultField ToField(FoundationDbColumn column)
    {
        return new DbQueryResultField(column.ColumnName, column.DataType, column.AllowDbNull == true);
    }

    #endregion

    private class Result
    {
        public readonly ReadOnlyCollection<DbQueryResultField> Fields;
        public Result(ReadOnlyCollection<DbQueryResultField> fields) => Fields = fields;
    }
}