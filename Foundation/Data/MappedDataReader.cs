using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Foundation.Assertions;

namespace Foundation.Data;

public delegate int GetValues(object[] values);

public class MappedDataReader : DbDataReader
{
    private readonly IDataReader _dataReader;
    private readonly GetValues _getValues;

    public MappedDataReader(IDataReader dataReader, GetValues getValues)
    {
        Assert.IsNotNull(dataReader);
        Assert.IsNotNull(getValues);

        _dataReader = dataReader;
        _getValues = getValues;
    }

    public override void Close() => _dataReader.Close();

    public override int Depth => _dataReader.Depth;

    public override int FieldCount => _dataReader.FieldCount;

    public override bool GetBoolean(int ordinal) => throw new NotImplementedException();

    public override byte GetByte(int ordinal) => throw new NotImplementedException();

    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override char GetChar(int ordinal) => throw new NotImplementedException();

    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override string GetDataTypeName(int ordinal) => throw new NotImplementedException();

    public override DateTime GetDateTime(int ordinal) => throw new NotImplementedException();

    public override decimal GetDecimal(int ordinal) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override double GetDouble(int ordinal)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override Type GetFieldType(int ordinal)
    {
        return _dataReader.GetFieldType(ordinal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    public override float GetFloat(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override Guid GetGuid(int ordinal) => throw new NotImplementedException();
    public override short GetInt16(int ordinal) => throw new NotImplementedException();
    public override int GetInt32(int ordinal) => throw new NotImplementedException();
    public override long GetInt64(int ordinal) => throw new NotImplementedException();
    public override string GetName(int ordinal) => _dataReader.GetName(ordinal);
    public override int GetOrdinal(string name) => _dataReader.GetOrdinal(name);
    public override DataTable GetSchemaTable() => _dataReader.GetSchemaTable();
    public override string GetString(int ordinal) => throw new NotImplementedException();
    public override object GetValue(int ordinal) => throw new NotImplementedException();
    public override int GetValues(object[] values) => _getValues(values);
    public override bool HasRows => throw new NotImplementedException();
    public override bool IsClosed => _dataReader.IsClosed;
    public override bool IsDBNull(int ordinal) => _dataReader.IsDBNull(ordinal);
    public override bool NextResult() => _dataReader.NextResult();
    public override bool Read() => _dataReader.Read();
    public override int RecordsAffected => _dataReader.RecordsAffected;
    public override object this[string name] => throw new NotImplementedException();
    public override object this[int ordinal] => throw new NotImplementedException();
}