using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using Foundation.Assertions;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
public sealed class TextDataReader : DbDataReader
{
    private readonly TextDataCommand _command;
    private readonly CommandBehavior _behavior;
    private readonly TextDataColumnCollection _columns;
    private DataTable _schemaTable;
    private readonly TextReader _textReader;
    private readonly TextDataStreamReader _textDataStreamReader;
    private object[] _values;
    private int _rowCount;

    internal TextDataReader(TextDataCommand command, CommandBehavior behavior)
    {
        ArgumentNullException.ThrowIfNull(command);

        _command = command;
        _behavior = behavior;
        var parameters = command.Parameters;

        Assert.IsTrue(parameters != null);

        _columns = parameters.GetParameterValue<TextDataColumnCollection>("columns");
        var converters = parameters.GetParameterValue<IList<ITextDataConverter>>("converters");
        var getTextReader = parameters.GetParameterValue<IConverter<TextDataCommand, TextReader>>("getTextReader");
        _textReader = getTextReader.Convert(command);
        _textDataStreamReader = new TextDataStreamReader(_textReader, _columns, converters);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Close()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public override int Depth => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override int FieldCount => _columns.Count;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override bool GetBoolean(int ordinal)
    {
        return (bool)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override byte GetByte(int ordinal)
    {
        return (byte)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <param name="dataOffset"></param>
    /// <param name="buffer"></param>
    /// <param name="bufferOffset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override char GetChar(int ordinal)
    {
        return (char)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <param name="dataOffset"></param>
    /// <param name="buffer"></param>
    /// <param name="bufferOffset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    public override string GetDataTypeName(int ordinal) => throw new NotImplementedException();

    public override DateTime GetDateTime(int ordinal) => (DateTime)_values[ordinal];

    public override decimal GetDecimal(int ordinal) => (decimal)_values[ordinal];

    public override double GetDouble(int ordinal) => (double)_values[ordinal];

    public override IEnumerator GetEnumerator() => throw new NotImplementedException();

    public override Type GetFieldType(int ordinal) => throw new NotImplementedException();

    public override float GetFloat(int ordinal) => (float)_values[ordinal];

    public override Guid GetGuid(int ordinal) => (Guid)_values[ordinal];

    public override short GetInt16(int ordinal) => (short)_values[ordinal];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override int GetInt32(int ordinal)
    {
        return (int)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override long GetInt64(int ordinal)
    {
        return (long)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override string GetName(int ordinal)
    {
        var column = _columns[ordinal];
        return column.ColumnName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override int GetOrdinal(string name)
    {
        return _columns.IndexOf(name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override DataTable GetSchemaTable()
    {
        if (_schemaTable == null)
        {
            _schemaTable = new DataTable();
            _schemaTable.Locale = CultureInfo.InvariantCulture;
            _schemaTable.Columns.Add("ColumnName", typeof(string));
            _schemaTable.Columns.Add("DataType", typeof(Type));
            _schemaTable.Columns.Add("IsKey", typeof(bool));

            foreach (var column in _columns)
            {
                object[] values =
                [
                    column.ColumnName,
                    column.DataType,
                    false
                ];

                _schemaTable.Rows.Add(values);
            }
        }

        return _schemaTable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override string GetString(int ordinal)
    {
        return (string)_values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override object GetValue(int ordinal)
    {
        return _values[ordinal];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public override int GetValues(object[] values)
    {
        _values.CopyTo(values, 0);
        return values.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool HasRows => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public override bool IsClosed => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override bool IsDBNull(int ordinal)
    {
        return _values[ordinal] == DBNull.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool NextResult()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Read()
    {
        bool read;

        if (_behavior == CommandBehavior.SingleRow && _rowCount == 1)
        {
            read = false;
        }
        else
        {
            _values = _textDataStreamReader.ReadRow();
            read = _values != null;

            if (read)
            {
                _rowCount++;
            }
        }

        return read;
    }

    /// <summary>
    /// 
    /// </summary>
    public override int RecordsAffected => _rowCount;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override object this[string name]
    {
        get
        {
            var index = _columns.IndexOf(name, true);
            return _values[index];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override object this[int ordinal] => _values[ordinal];
}