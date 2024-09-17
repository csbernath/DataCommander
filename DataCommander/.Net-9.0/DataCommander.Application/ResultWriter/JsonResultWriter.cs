using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using Foundation.Data;
using Newtonsoft.Json;

namespace DataCommander.Application.ResultWriter;

public class JsonResultWriter(Action<InfoMessage> addInfoMessage) : IResultWriter
{
    private readonly Action<InfoMessage> _addInfoMessage = addInfoMessage;
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage);
    private Guid? _guid;
    private int _tableIndex;    
    private List<FoundationDbColumn> _columns;
    private JsonTextWriter _jsonTextWriter;

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

        string path = Path.Combine(Path.GetTempPath(), $"JsonResult {_guid} {_tableIndex}.json");
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, $"Creating file {path}..."));

        StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);
        _jsonTextWriter = new JsonTextWriter(streamWriter)
        {
            Formatting = Formatting.Indented
        };
        _jsonTextWriter.WriteStartArray();

        _columns = schemaTable.Rows.Cast<DataRow>().Select(FoundationDbColumnFactory.Create).ToList();
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);

        for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
        {
            object[] row = rows[rowIndex];

            _jsonTextWriter.WriteStartObject();

            for (int columnIndex = 0; columnIndex < row.Length; ++columnIndex)
            {
                FoundationDbColumn column = _columns[columnIndex];
                object value = row[columnIndex];

                _jsonTextWriter.WritePropertyName(column.ColumnName);

                switch (value)
                {
                    case DateTimeField dateTimeField:
                        value = dateTimeField.Value;
                        break;

                    case BinaryField binaryField:
                        value = binaryField.Value;
                        break;

                    default:
                        break;
                }

                _jsonTextWriter.WriteValue(value);
            }

            _jsonTextWriter.WriteEndObject();
        }
    }

    void IResultWriter.WriteTableEnd()
    {
        _logResultWriter.WriteTableEnd();

        _jsonTextWriter.WriteEndArray();
        _jsonTextWriter.Close();
        _jsonTextWriter = null;

        ++_tableIndex;
    }
}