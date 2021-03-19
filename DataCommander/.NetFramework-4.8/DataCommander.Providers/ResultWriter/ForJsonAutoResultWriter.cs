using System;
using System.Data;
using System.IO;
using System.Text;
using DataCommander.Providers2.Connection;
using DataCommander.Providers2.FieldNamespace;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Newtonsoft.Json;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class ForJsonAutoResultWriter : IResultWriter
    {
        private readonly Action<InfoMessage> _addInfoMessage;
        private readonly IResultWriter _logResultWriter;
        private bool isJsonAuto;
        private string _path;
        private TextWriter _textWriter;
        private string _formattedPath;

        public ForJsonAutoResultWriter(Action<InfoMessage> addInfoMessage)
        {
            _addInfoMessage = addInfoMessage;
            _logResultWriter = new LogResultWriter(addInfoMessage);
        }

        void IResultWriter.Begin(IProvider provider)
        {
            _logResultWriter.Begin(provider);
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            _logResultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            _logResultWriter.AfterExecuteReader(fieldCount);
            isJsonAuto = fieldCount == 1;
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            _logResultWriter.AfterCloseReader(affectedRows);
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _logResultWriter.WriteTableBegin(schemaTable);

            if (isJsonAuto)
            {
                var column = FoundationDbColumnFactory.Create(schemaTable.Rows[0]);
                isJsonAuto = column.DataType == typeof(string);
            }

            if (isJsonAuto)
            {
                Assert.IsNull(_path);

                var identifier = LocalTime.Default.Now.ToString("yyyy-MM-dd HHmmss.fff");
                var tempPath = Path.GetTempPath();
                _path = Path.Combine(tempPath, identifier + ".json");
                _textWriter = new StreamWriter(_path, false, Encoding.UTF8);
                _formattedPath = Path.Combine(tempPath, identifier + "formatted.json");
                _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, $"Formatted JSON file: {_formattedPath}"));
            }
            else
                _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Error, null, "This is not FOR JSON AUTO"));
        }

        void IResultWriter.FirstRowReadBegin()
        {
            _logResultWriter.FirstRowReadBegin();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            _logResultWriter.FirstRowReadEnd(dataTypeNames);
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            _logResultWriter.WriteRows(rows, rowCount);

            if (isJsonAuto)
            {
                for (var rowIndex = 0; rowIndex < rowCount; ++rowIndex)
                {
                    var row = rows[rowIndex];
                    var stringField = (StringField) row[0];
                    var fragment = stringField.Value;
                    _textWriter.Write(fragment);
                }
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            _logResultWriter.WriteTableEnd();

            if (isJsonAuto)
            {
                _textWriter.Close();
                _textWriter = null;

                using (var streamReader = new StreamReader(_path))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(_formattedPath)))
                        {
                            jsonTextWriter.Formatting = Formatting.Indented;
                            while (true)
                            {
                                var read = jsonTextReader.Read();
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
                                        var propertyName = (string) jsonTextReader.Value;
                                        jsonTextWriter.WritePropertyName(propertyName);
                                        break;
                                    case JsonToken.Comment:
                                        break;
                                    case JsonToken.Raw:
                                        break;
                                    case JsonToken.Integer:
                                    {
                                        var value = jsonTextReader.Value;
                                        jsonTextWriter.WriteValue(value);
                                    }
                                        break;
                                    case JsonToken.Float:
                                    {
                                        var value = jsonTextReader.Value;
                                        jsonTextWriter.WriteValue(value);
                                    }
                                        break;
                                    case JsonToken.String:
                                    {
                                        var value = jsonTextReader.Value;
                                        jsonTextWriter.WriteValue(value);
                                    }
                                        break;
                                    case JsonToken.Boolean:
                                    {
                                        var value = jsonTextReader.Value;
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
                                        var value = jsonTextReader.Value;
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

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
            _logResultWriter.WriteParameters(parameters);
        }

        void IResultWriter.End()
        {
            _logResultWriter.End();
        }
    }
}