using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;

namespace Foundation.Data.TextData;

public sealed class TextDataReader : DbDataReader
{
    private readonly TextDataCommand _command;
    private readonly CommandBehavior _behavior;
    private readonly TextDataColumnCollection _columns;
    private DataTable? _schemaTable;
    private readonly TextReader _textReader;
    private readonly TextDataStreamReader _textDataStreamReader;
    private object[]? _values;
    private int _rowCount;

    internal TextDataReader(TextDataCommand command, CommandBehavior behavior)
    {
        ArgumentNullException.ThrowIfNull(command);

        _command = command;
        _behavior = behavior;
        var parameters = command.Parameters;

        ArgumentNullException.ThrowIfNull(parameters);

        _columns = parameters.GetParameterValue<TextDataColumnCollection>("columns")!;
        var converters = parameters.GetParameterValue<IList<ITextDataConverter>>("converters")!;
        var getTextReader = parameters.GetParameterValue<IConverter<TextDataCommand, TextReader>>("getTextReader")!;
        _textReader = getTextReader.Convert(command);
        _textDataStreamReader = new TextDataStreamReader(_textReader, _columns, converters);
    }

    public override void Close()
    {
    }

    public override int Depth => throw new NotImplementedException();

    public override int FieldCount => _columns.Count;

    public override bool GetBoolean(int ordinal) => (bool)_values![ordinal];

    public override byte GetByte(int ordinal) => (byte)_values![ordinal];

    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override char GetChar(int ordinal) => (char)_values![ordinal];

    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override string GetDataTypeName(int ordinal) => throw new NotImplementedException();

    public override DateTime GetDateTime(int ordinal) => (DateTime)_values![ordinal];

    public override decimal GetDecimal(int ordinal) => (decimal)_values![ordinal];

    public override double GetDouble(int ordinal) => (double)_values![ordinal];

    public override IEnumerator GetEnumerator() => throw new NotImplementedException();

    public override Type GetFieldType(int ordinal) => throw new NotImplementedException();

    public override float GetFloat(int ordinal) => (float)_values![ordinal];

    public override Guid GetGuid(int ordinal) => (Guid)_values![ordinal];

    public override short GetInt16(int ordinal) => (short)_values![ordinal];

    public override int GetInt32(int ordinal) => (int)_values![ordinal];

    public override long GetInt64(int ordinal) => (long)_values![ordinal];

    public override string GetName(int ordinal)
    {
        var column = _columns[ordinal];
        return column.ColumnName;
    }

    public override int GetOrdinal(string name) => _columns.IndexOf(name);

    public override DataTable GetSchemaTable()
    {
        if (_schemaTable == null)
        {
            _schemaTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
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

    public override string GetString(int ordinal) => (string)_values![ordinal];

    public override object GetValue(int ordinal) => _values![ordinal];

    public override int GetValues(object[] values)
    {
        _values!.CopyTo(values, 0);
        return values.Length;
    }

    public override bool HasRows => throw new NotImplementedException();

    public override bool IsClosed => throw new NotImplementedException();

    public override bool IsDBNull(int ordinal) => _values![ordinal] == DBNull.Value;

    public override bool NextResult() => throw new NotImplementedException();

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

    public override int RecordsAffected => _rowCount;

    public override object this[string name]
    {
        get
        {
            var index = _columns.IndexOf(name, true);
            return _values![index];
        }
    }

    public override object this[int ordinal] => _values![ordinal];
}