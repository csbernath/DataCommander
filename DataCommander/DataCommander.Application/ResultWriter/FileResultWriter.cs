﻿using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using DataCommander.Api;
using Foundation.Text;

namespace DataCommander.Application.ResultWriter;

internal sealed class FileResultWriter : IResultWriter
{
    private readonly TextWriter _messageWriter;
    private StreamWriter? _streamWriter;
    private DataWriterBase[]? _dataWriters;

    public FileResultWriter(TextWriter messageWriter)
    {
        ArgumentNullException.ThrowIfNull(messageWriter);

        _messageWriter = messageWriter;
    }

    void IResultWriter.Begin(IProvider provider)
    {
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader()
    {
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        var path = Path.GetTempFileName();
        _messageWriter.WriteLine("fileName: {0}", path);
        var encoding = Encoding.UTF8;
        _streamWriter = new StreamWriter(path, false, encoding, 4096)
        {
            AutoFlush = true
        };
        var count = schemaTable.Rows.Count;
        _dataWriters = new DataWriterBase[count];
        var st = new StringTable(3);
        st.Columns[2].Align = StringTableColumnAlign.Right;

        for (var i = 0; i < count; i++)
        {
            DataWriterBase? dataWriter = null;
            var column = schemaTable.Rows[i];
            var dataType = (Type)column["DataType"];
            var typeCode = Type.GetTypeCode(dataType);
            string dataTypeName;
            int length;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    length = 1;
                    dataTypeName = "bit";
                    dataWriter = new BooleanDataWriter();
                    dataWriter.Init(5);
                    break;

                case TypeCode.DateTime:
                    length = 21; // yyyyMMdd HH:mm:ss.fff
                    dataTypeName = "datetime";
                    dataWriter = new DateTimeDataWriter();
                    dataWriter.Init(21);
                    break;

                case TypeCode.Decimal:
                    var precision = (short)column["NumericPrecision"];
                    var scale = (short)column["NumericScale"];
                    length = precision + 1; // +/- sign

                    // decimal separator
                    if (scale > 0)
                    {
                        length++;
                        dataTypeName = $"decimal({precision},{scale})";
                    }
                    else
                    {
                        dataTypeName = $"decimal({precision})";
                    }

                    dataWriter = new DecimalDataWriter();
                    dataWriter.Init(length);
                    break;

                case TypeCode.Int16:
                    length = short.MinValue.ToString().Length;
                    dataTypeName = "smallint";
                    dataWriter = new DecimalDataWriter();
                    dataWriter.Init(length);
                    break;

                case TypeCode.Int32:
                    length = int.MinValue.ToString().Length;
                    dataTypeName = "int";
                    dataWriter = new DecimalDataWriter();
                    dataWriter.Init(length);
                    break;

                case TypeCode.String:
                    length = (int)column["ColumnSize"];
                    length = Math.Min(1024, length);
                    dataTypeName = $"varchar({length})";
                    dataWriter = new StringDataWriter();
                    dataWriter.Init(length);
                    break;

                case TypeCode.Object:
                    if (dataType == typeof(Guid))
                    {
                        length = Guid.Empty.ToString().Length;
                        dataTypeName = "uniqueidentifier";
                    }
                    else
                    {
                        throw new NotImplementedException(dataType.ToString());
                    }

                    break;

                default:
                    throw new NotImplementedException(typeCode.ToString());
            }

            _dataWriters[i] = dataWriter!;

            var row = st.NewRow();
            row[0] = (string)column[SchemaTableColumn.ColumnName];
            row[1] = dataTypeName;
            row[2] = length.ToString();
            st.Rows.Add(row);
        }

        _messageWriter.WriteLine(st);
    }

    public void FirstRowReadBegin()
    {
    }

    public void FirstRowReadEnd(string[] dataTypeNames)
    {
    }

    public void WriteRows(object[][] rows, int rowCount)
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < rowCount; i++)
        {
            var row = rows[i];
            for (var j = 0; j < row.Length; j++)
            {
                var s = _dataWriters![j].ToString(row[j]);
                stringBuilder.Append(s);
            }

            stringBuilder.Append("\r\n");
        }

        _streamWriter!.Write(stringBuilder);
    }

    public void WriteTableEnd()
    {
        _streamWriter!.Close();
        _streamWriter = null;
        _dataWriters = null;
    }

    public void WriteParameters(IDataParameterCollection parameters)
    {
        // TODO:  Add FileResultWriter.WriteParameters implementation
    }

    public static void WriteEnd()
    {
        // TODO:  Add FileResultWriter.WriteEnd implementation
    }

    void IResultWriter.End()
    {
    }
}