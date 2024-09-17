using System;
using System.Data;
using System.IO;
using System.Text;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Newtonsoft.Json;

namespace DataCommander.Application.ResultWriter;

internal sealed class ForJsonAutoResultWriter(Action<InfoMessage> addInfoMessage) : IResultWriter
{
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage);
    private bool _isJsonAuto;
    private string _path;
    private TextWriter _textWriter;
    private string _formattedPath;

    void IResultWriter.Begin(IProvider provider)
    {
        _logResultWriter.Begin(provider);
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
    {
        _logResultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
    }

    void IResultWriter.AfterExecuteReader() => _logResultWriter.AfterExecuteReader();
    void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        _logResultWriter.WriteTableBegin(schemaTable);
        _isJsonAuto = schemaTable.Rows.Count == 1;

        if (_isJsonAuto)
        {
            FoundationDbColumn column = FoundationDbColumnFactory.Create(schemaTable.Rows[0]);
            _isJsonAuto = column.DataType == typeof(string);
        }

        if (_isJsonAuto)
        {
            Assert.IsNull(_path);

            string identifier = LocalTime.Default.Now.ToString("yyyy-MM-dd HHmmss.fff");
            string tempPath = Path.GetTempPath();
            _path = Path.Combine(tempPath, identifier + ".json");
            _textWriter = new StreamWriter(_path, false, Encoding.UTF8);
            _formattedPath = Path.Combine(tempPath, identifier + "formatted.json");
            addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, $"Formatted JSON file: {_formattedPath}"));
        }
        else
            addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Error, null, "This is not FOR JSON AUTO"));
    }

    void IResultWriter.FirstRowReadBegin() => _logResultWriter.FirstRowReadBegin();
    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);

        if (_isJsonAuto)
        {
            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                object[] row = rows[rowIndex];
                StringField stringField = (StringField)row[0];
                string fragment = stringField.Value;
                _textWriter.Write(fragment);
            }
        }
    }

    void IResultWriter.WriteTableEnd()
    {
        _logResultWriter.WriteTableEnd();

        if (_isJsonAuto)
        {
            _textWriter.Close();
            _textWriter = null;

            using (StreamReader streamReader = new StreamReader(_path))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                {
                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(_formattedPath)))
                    {
                        jsonTextWriter.Formatting = Formatting.Indented;
                        while (true)
                        {
                            bool read = jsonTextReader.Read();
                            if (!read)
                                break;

                            switch (jsonTextReader.TokenType)
                            {
                                case JsonToken.None:
                                    break;
                                case JsonToken.StartObject:
                                    jsonTextWriter.WriteStartObject();
                                    break;
                                case JsonToken.StartArray:
                                    jsonTextWriter.WriteStartArray();
                                    break;
                                case JsonToken.StartConstructor:
                                    break;
                                case JsonToken.PropertyName:
                                    string? propertyName = (string)jsonTextReader.Value;
                                    jsonTextWriter.WritePropertyName(propertyName);
                                    break;
                                case JsonToken.Comment:
                                    break;
                                case JsonToken.Raw:
                                    break;
                                case JsonToken.Integer:
                                {
                                        object? value = jsonTextReader.Value;
                                    jsonTextWriter.WriteValue(value);
                                }
                                    break;
                                case JsonToken.Float:
                                {
                                        object? value = jsonTextReader.Value;
                                    jsonTextWriter.WriteValue(value);
                                }
                                    break;
                                case JsonToken.String:
                                {
                                        object? value = jsonTextReader.Value;
                                    jsonTextWriter.WriteValue(value);
                                }
                                    break;
                                case JsonToken.Boolean:
                                {
                                        object? value = jsonTextReader.Value;
                                    jsonTextWriter.WriteValue(value);
                                }
                                    break;
                                case JsonToken.Null:
                                    break;
                                case JsonToken.Undefined:
                                    break;
                                case JsonToken.EndObject:
                                    jsonTextWriter.WriteEndObject();
                                    break;
                                case JsonToken.EndArray:
                                    jsonTextWriter.WriteEndArray();
                                    break;
                                case JsonToken.EndConstructor:
                                    break;
                                case JsonToken.Date:
                                {
                                        object? value = jsonTextReader.Value;
                                    jsonTextWriter.WriteValue(value);
                                }
                                    break;
                                case JsonToken.Bytes:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }
        }
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters) => _logResultWriter.WriteParameters(parameters);
    void IResultWriter.End() => _logResultWriter.End();
}