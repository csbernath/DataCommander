using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection;

internal sealed class LoggedDataReader : IDataReader
{
    private readonly IDataReader _dataReader;
    private readonly EventHandler<AfterReadEventArgs>? _afterRead;
    private int _rowCount;

    public LoggedDataReader(IDataReader dataReader, EventHandler<AfterReadEventArgs>? afterRead)
    {
        ArgumentNullException.ThrowIfNull(dataReader);

        _dataReader = dataReader;
        _afterRead = afterRead;
    }

    void IDataReader.Close() => _dataReader.Close();
    int IDataReader.Depth => _dataReader.Depth;
    DataTable? IDataReader.GetSchemaTable() => _dataReader.GetSchemaTable();
    bool IDataReader.IsClosed => _dataReader.IsClosed;
    bool IDataReader.NextResult() => _dataReader.NextResult();

    bool IDataReader.Read()
    {
        var read = _dataReader.Read();
        if (read)
        {
            ++_rowCount;
        }
        else if (_afterRead != null)
        {
            var eventArgs = new AfterReadEventArgs(_rowCount);
            _afterRead(this, eventArgs);
        }

        return read;
    }

    int IDataReader.RecordsAffected => _dataReader.RecordsAffected;

    void IDisposable.Dispose() => _dataReader.Dispose();

    int IDataRecord.FieldCount => _dataReader.FieldCount;
    bool IDataRecord.GetBoolean(int i) => _dataReader.GetBoolean(i);
    byte IDataRecord.GetByte(int i) => _dataReader.GetByte(i);

    long IDataRecord.GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) =>
        _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    char IDataRecord.GetChar(int i) => _dataReader.GetChar(i);

    long IDataRecord.GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) =>
        _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    IDataReader IDataRecord.GetData(int i) => _dataReader.GetData(i);
    string IDataRecord.GetDataTypeName(int i) => _dataReader.GetDataTypeName(i);
    DateTime IDataRecord.GetDateTime(int i) => _dataReader.GetDateTime(i);
    decimal IDataRecord.GetDecimal(int i) => _dataReader.GetDecimal(i);
    double IDataRecord.GetDouble(int i) => _dataReader.GetDouble(i);
    Type IDataRecord.GetFieldType(int i) => _dataReader.GetFieldType(i);
    float IDataRecord.GetFloat(int i) => _dataReader.GetFloat(i);
    Guid IDataRecord.GetGuid(int i) => _dataReader.GetGuid(i);
    short IDataRecord.GetInt16(int i) => _dataReader.GetInt16(i);
    int IDataRecord.GetInt32(int i) => _dataReader.GetInt32(i);
    long IDataRecord.GetInt64(int i) => _dataReader.GetInt64(i);
    string IDataRecord.GetName(int i) => _dataReader.GetName(i);
    int IDataRecord.GetOrdinal(string name) => _dataReader.GetOrdinal(name);
    string IDataRecord.GetString(int i) => _dataReader.GetString(i);
    object IDataRecord.GetValue(int i) => _dataReader.GetValue(i);
    int IDataRecord.GetValues(object[] values) => _dataReader.GetValues(values);
    bool IDataRecord.IsDBNull(int i) => _dataReader.IsDBNull(i);
    object IDataRecord.this[string name] => _dataReader[name];
    object IDataRecord.this[int i] => _dataReader[i];
}