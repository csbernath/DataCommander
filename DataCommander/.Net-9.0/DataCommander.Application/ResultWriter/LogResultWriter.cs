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
using Foundation.Collections;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Data.SqlClient.DbQueryBuilding;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;

namespace DataCommander.Application.ResultWriter;

internal sealed class LogResultWriter : IResultWriter
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly Action<InfoMessage> _addInfoMessage;
    private IProvider _provider;
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

    void IResultWriter.Begin(IProvider provider)
    {
        _provider = provider;
        _beginTimestamp = Stopwatch.GetTimestamp();
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
    {
        _beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
        ++_commandCount;

        System.Data.Common.DbCommand command = asyncDataAdapterCommand.Command;
        const long duration = 0;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        string message = $"Before executing reader at line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";
        System.Data.Common.DbParameterCollection parameters = command.Parameters;
        if (!parameters.IsNullOrEmpty())
            message += "\r\n" + command.Parameters.ToLogString();

        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        _query = asyncDataAdapterCommand.Query;
        if (_query != null)
        {
            _fileName = asyncDataAdapterCommand.FileName;
            _parameters = asyncDataAdapterCommand.Parameters;
            _commandText = asyncDataAdapterCommand.CommandText;
            _results = [];
        }
    }

    void IResultWriter.AfterExecuteReader()
    {
        long duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount - 1}]";
        string message = "Executing reader...";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        _tableCount = 0;
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
        long duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"Reader closed.");
        if (affectedRows >= 0)
            stringBuilder.Append($" {StringExtensions.SingularOrPlural(affectedRows, "row", "rows")} affected.");
        string message = stringBuilder.ToString();
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        if (_query != null)
        {
            string? directory = _fileName != null ? Path.GetDirectoryName(_fileName) : Path.GetTempPath();
            ReadOnlyCollection<DbQueryResult> results = _query.Results.EmptyIfNull().Zip(_results, ToResult).ToReadOnlyCollection();
            DbRequest query = new DbRequest(directory, _query.Name, _query.Using, _query.Namespace, _commandText, 0, _parameters, results);

            DbRequestBuilder queryBuilder = new DbRequestBuilder(query);
            string csharpSourceCode = queryBuilder.Build();

            string path = Path.Combine(query.Directory, $"{query.Name}.generated.cs");
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

        long duration = _writeTableBeginTimestamp - _beforeExecuteReaderTimestamp;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        string message =
            $"Result[{_tableCount - 1}] has {StringExtensions.SingularOrPlural(schemaTable.Rows.Count, "column", "columns")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));

        Log.Trace($"SchemaTable of table[{_tableCount - 1}], {schemaTable.TableName}:\r\n{schemaTable.ToStringTableString()}");

        FoundationDbColumn[] dbColumns = schemaTable.Rows.Cast<DataRow>().Select(FoundationDbColumnFactory.Create).ToArray();

        if (_provider.Identifier == ProviderIdentifier.SqlServer)
        {
            TextBuilder declareTableScript = ToDeclareTableScript(schemaTable.TableName, dbColumns);
            message = $"\r\n{declareTableScript.ToLines().ToIndentedString("    ")}";
            _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, string.Empty, message));
        }

        List<DataTransferObjectField> dataTransferObjectFields = dbColumns.Select(ToDataTransferObjectField).ToList();
        ReadOnlyCollection<Line> dataTransferObject = DataTransferObjectWithPropertiesFactory.Create(schemaTable.TableName, dataTransferObjectFields);
        message = $"\r\n{dataTransferObject.ToIndentedString("    ")}";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, string.Empty, message));
        
        if (_query != null)
        {
            ReadOnlyCollection<DbQueryResultField> fields = dbColumns.Select(ToField).ToReadOnlyCollection();
            Result result = new Result(fields);
            _results.Add(result);
        }
    }

    private static TextBuilder ToDeclareTableScript(string tableName, IReadOnlyList<FoundationDbColumn> dbColumns)
    {
        Dictionary<SqlDbType, SqlDataType> sqlDataTypes = SqlDataTypeRepository.SqlDataTypes.ToDictionary(t => t.SqlDbType);
        TextBuilder textBuilder = new TextBuilder();
        textBuilder.Add($"declare @{tableName} table");
        using (textBuilder.AddBlock("(", ")"))
        {
            for (int columnIndex = 0; columnIndex < dbColumns.Count; ++columnIndex)
            {
                FoundationDbColumn dbColumn = dbColumns[columnIndex];
                StringBuilder columnScript = ToDeclareTableColumnScript(dbColumn, sqlDataTypes);

                if (columnIndex < dbColumns.Count - 1)
                    columnScript.Append(',');

                textBuilder.Add(columnScript.ToString());
            }
        }

        return textBuilder;
    }

    private static StringBuilder ToDeclareTableColumnScript(
        FoundationDbColumn dbColumn,
        IReadOnlyDictionary<SqlDbType, SqlDataType> sqlDataTypes)
    {
        SqlDbType sqlDbType = (SqlDbType)dbColumn.ProviderType;
        SqlDataType sqlDataType = sqlDataTypes[sqlDbType];

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(dbColumn.ColumnName);
        stringBuilder.Append(' ');
        stringBuilder.Append(sqlDataType.SqlDataTypeName);

        switch (sqlDbType)
        {
            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.NVarChar:
            case SqlDbType.VarChar:
                stringBuilder.Append('(');
                stringBuilder.Append(dbColumn.ColumnSize);
                stringBuilder.Append(')');
                break;

            case SqlDbType.Decimal:
                stringBuilder.Append('(');
                stringBuilder.Append(dbColumn.NumericPrecision.Value);

                if (dbColumn.NumericScale > 0)
                {
                    stringBuilder.Append(',');
                    stringBuilder.Append(dbColumn.NumericScale.Value);
                }

                stringBuilder.Append(')');
                break;
        }

        if (dbColumn.AllowDbNull == false)
            stringBuilder.Append(" not null");

        return stringBuilder;
    }

    private DataTransferObjectField ToDataTransferObjectField(FoundationDbColumn dbColumn)
    {
        string name = dbColumn.ColumnName;

        CSharpType cSharpType = CSharpTypeArray.CSharpTypes.First(t => t.Type == dbColumn.DataType);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(cSharpType.Name);

        if (dbColumn.AllowDbNull == true && cSharpType.Type != typeof(string))
            stringBuilder.Append('?');

        string type = stringBuilder.ToString();
        
        return new DataTransferObjectField(name, type);
    }

    void IResultWriter.FirstRowReadBegin() => _firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
        long duration = Stopwatch.GetTimestamp() - _firstRowReadBeginTimestamp;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount-1}]";
        string message = $"Result[{_tableCount - 1}] first row read completed.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount) => _rowCount += rowCount;

    void IResultWriter.WriteTableEnd()
    {
        long duration = Stopwatch.GetTimestamp() - _writeTableBeginTimestamp;
        string header = $"{StopwatchTimeSpan.ToString(duration, 3)} Command[{_commandCount - 1}]";
        string message =
            $"Result[{_tableCount - 1}] has {StringExtensions.SingularOrPlural(_rowCount, "row", "rows")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
    }

    void IResultWriter.End()
    {
        long duration = Stopwatch.GetTimestamp() - _beginTimestamp;
        string header = StopwatchTimeSpan.ToString(duration, 3);
        string message = $"Query completed {StringExtensions.SingularOrPlural(_commandCount, "command", "commands")}.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, header, message));
    }

    private static DbQueryResult ToResult(string result, Result sql)
    {
        Parser.ParseResult(result, out string? name, out string? fieldName);
        return new DbQueryResult(name, fieldName, sql.Fields);
    }

    private static DbQueryResultField ToField(FoundationDbColumn column)
    {
        return new DbQueryResultField(column.ColumnName, column.DataType, column.AllowDbNull == true);
    }

    private class Result(ReadOnlyCollection<DbQueryResultField> fields)
    {
        public readonly ReadOnlyCollection<DbQueryResultField> Fields = fields;
    }
}