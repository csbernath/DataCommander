using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Providers2.Connection;
using DataCommander.Providers2.QueryConfiguration;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.DbQueryBuilding;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers.ResultWriter;

internal sealed class LogResultWriter : IResultWriter
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly Action<InfoMessage> _addInfoMessage;
    private int _commandCount;
    private int _tableCount;
    private int _fieldCount;
    private int _rowCount;
    private long _beginTimestamp;
    private long _beforeExecuteReaderTimestamp;
    private long _writeTableBeginTimestamp;
    private long _firstRowReadBeginTimestamp;

    private string _fileName;
    private Providers2.QueryConfiguration.Query _query;
    private ReadOnlyCollection<DbRequestParameter> _parameters;
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
        var message = $"Command[{_commandCount - 1}] executing from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";
        var parameters = command.Parameters;
        if (!parameters.IsNullOrEmpty())
            message += "\r\n" + command.Parameters.ToLogString();

        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message));

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

        var header = StopwatchTimeSpan.ToString(duration, 3);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Command[{_commandCount - 1}] started.");
        if (fieldCount > 0)
            stringBuilder.Append($" Field count: {fieldCount}");
        var message = stringBuilder.ToString();
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        _tableCount = 0;
        _fieldCount = fieldCount;
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
        var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
        var header = StopwatchTimeSpan.ToString(duration, 3);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Command[{_commandCount - 1}] completed.");
        if (affectedRows >= 0)
            stringBuilder.Append($" {affectedRows} row(s) affected.");
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

        ++_tableCount;
        _rowCount = 0;
    }

    void IResultWriter.FirstRowReadBegin() => _firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
        var duration = Stopwatch.GetTimestamp() - _firstRowReadBeginTimestamp;
        var header = StopwatchTimeSpan.ToString(duration, 3);
        var message =
            $"Command[{_commandCount - 1}] result[{_tableCount - 1}] first row read completed.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount) => _rowCount += rowCount;

    void IResultWriter.WriteTableEnd()
    {
        var duration = Stopwatch.GetTimestamp() - _writeTableBeginTimestamp;
        var header = StopwatchTimeSpan.ToString(duration, 3);
        var message =
            $"Command[{_commandCount - 1}] result[{_tableCount - 1}] finished. Table[{_tableCount - 1},{_query?.Results[_tableCount - 1]}] has {_fieldCount} column(s), {_rowCount} row(s).";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
    }

    void IResultWriter.End()
    {
        var duration = Stopwatch.GetTimestamp() - _beginTimestamp;
        var header = StopwatchTimeSpan.ToString(duration, 3);
        var message = $"Query completed {_commandCount} command(s).";
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