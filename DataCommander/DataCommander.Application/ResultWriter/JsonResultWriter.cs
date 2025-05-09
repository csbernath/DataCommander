using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using Foundation.Data;

namespace DataCommander.Application.ResultWriter;

public class JsonResultWriter(Action<InfoMessage> addInfoMessage) : IResultWriter
{
    private readonly Action<InfoMessage> _addInfoMessage = addInfoMessage;
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage, false);
    private Guid? _guid;
    private int _tableIndex;    
    private List<FoundationDbColumn>? _columns;
    private FileStream? _utf8JsonStream;    
    private Utf8JsonWriter? _utf8JsonWriter;

    void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);
    void IResultWriter.AfterExecuteReader() => _logResultWriter.AfterExecuteReader();
    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand) => _logResultWriter.BeforeExecuteReader(asyncDataAdapterCommand);

    void IResultWriter.Begin(IProvider provider) => _logResultWriter.Begin(provider);
    void IResultWriter.End() => _logResultWriter.End();
    void IResultWriter.FirstRowReadBegin() => _logResultWriter.FirstRowReadBegin();
    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);
    void IResultWriter.WriteParameters(IDataParameterCollection parameters) => _logResultWriter.WriteParameters(parameters);

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        _logResultWriter.WriteTableBegin(schemaTable);

        if (_guid == null)
            _guid = Guid.NewGuid();

        var path = Path.Combine(Path.GetTempPath(), $"JsonResult {_guid} {_tableIndex}.json");
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, $"Creating file {path}..."));
        
        _utf8JsonStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        _utf8JsonWriter = new Utf8JsonWriter(_utf8JsonStream, new JsonWriterOptions { Indented = true });
        _utf8JsonWriter.WriteStartArray();

        _columns = schemaTable.Rows.Cast<DataRow>().Select(FoundationDbColumnFactory.Create).ToList();
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);

        for (var rowIndex = 0; rowIndex < rowCount; ++rowIndex)
        {
            var row = rows[rowIndex];

            _utf8JsonWriter!.WriteStartObject();

            for (var columnIndex = 0; columnIndex < row.Length; ++columnIndex)
            {
                var column = _columns![columnIndex];
                var value = row[columnIndex];

                _utf8JsonWriter.WritePropertyName(column.ColumnName!);

                switch (value)
                {
                    case DBNull:
                        _utf8JsonWriter.WriteNullValue();
                        break;
                    case bool boolValue:
                        _utf8JsonWriter.WriteBooleanValue(boolValue);
                        break;
                    case int int32Value:
                        _utf8JsonWriter.WriteNumberValue(int32Value);
                        break;
                    case long longValue:
                        _utf8JsonWriter.WriteNumberValue(longValue);
                        break;
                    case string stringValue:
                        _utf8JsonWriter.WriteStringValue(stringValue);
                        break;
                    case BinaryField binaryField:
                        _utf8JsonWriter.WriteStringValue(binaryField.Value);
                        break;
                    case DateTimeField dateTimeField:
                        _utf8JsonWriter.WriteStringValue(dateTimeField.Value);
                        break;
                    case StringField stringField:
                        _utf8JsonWriter.WriteStringValue(stringField.Value);
                        break;
                    default:
                        Debug.WriteLine("");
                        break;
                }
            }

            _utf8JsonWriter.WriteEndObject();
        }
    }

    void IResultWriter.WriteTableEnd()
    {
        _logResultWriter.WriteTableEnd();

        _utf8JsonWriter!.WriteEndArray();
        _utf8JsonWriter.Dispose();
        _utf8JsonWriter = null;
        
        //_utf8JsonStream!.Close();
        _utf8JsonStream!.Dispose();
        _utf8JsonStream = null;

        ++_tableIndex;
    }
}