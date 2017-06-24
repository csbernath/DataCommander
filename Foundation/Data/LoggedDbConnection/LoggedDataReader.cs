using System;
using System.Data;

namespace Foundation.Data.LoggedDbConnection
{
    internal sealed class LoggedDataReader : IDataReader
    {
        private readonly IDataReader _dataReader;
        private readonly EventHandler<AfterReadEventArgs> _afterRead;
        private int _rowCount;

        public LoggedDataReader(
            IDataReader dataReader,
            EventHandler<AfterReadEventArgs> afterRead)
        {
#if CONTRACTS_FULL
            Contract.Requires(dataReader != null);
            Contract.Requires(afterRead != null);
#endif

            _dataReader = dataReader;
            _afterRead = afterRead;
        }

#region IDataReader Members

        void IDataReader.Close()
        {
            _dataReader.Close();
        }

        int IDataReader.Depth => _dataReader.Depth;

        DataTable IDataReader.GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        bool IDataReader.IsClosed => _dataReader.IsClosed;

        bool IDataReader.NextResult()
        {
            return _dataReader.NextResult();
        }

        bool IDataReader.Read()
        {
            var read = _dataReader.Read();
            if (read)
            {
                _rowCount++;
            }
            else if (_afterRead != null)
            {
                var eventArgs = new AfterReadEventArgs(_rowCount);
                _afterRead(this, eventArgs);
            }

            return read;
        }

        int IDataReader.RecordsAffected => _dataReader.RecordsAffected;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            _dataReader.Dispose();
        }

#endregion

#region IDataRecord Members

        int IDataRecord.FieldCount => _dataReader.FieldCount;

        bool IDataRecord.GetBoolean(int i)
        {
            return _dataReader.GetBoolean(i);
        }

        byte IDataRecord.GetByte(int i)
        {
            return _dataReader.GetByte(i);
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        char IDataRecord.GetChar(int i)
        {
            return _dataReader.GetChar(i);
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            return _dataReader.GetDateTime(i);
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            return _dataReader.GetDecimal(i);
        }

        double IDataRecord.GetDouble(int i)
        {
            return _dataReader.GetDouble(i);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        float IDataRecord.GetFloat(int i)
        {
            return _dataReader.GetFloat(i);
        }

        Guid IDataRecord.GetGuid(int i)
        {
            return _dataReader.GetGuid(i);
        }

        short IDataRecord.GetInt16(int i)
        {
            return _dataReader.GetInt16(i);
        }

        int IDataRecord.GetInt32(int i)
        {
            return _dataReader.GetInt32(i);
        }

        long IDataRecord.GetInt64(int i)
        {
            return _dataReader.GetInt64(i);
        }

        string IDataRecord.GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        string IDataRecord.GetString(int i)
        {
            return _dataReader.GetString(i);
        }

        object IDataRecord.GetValue(int i)
        {
            return _dataReader.GetValue(i);
        }

        int IDataRecord.GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        bool IDataRecord.IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        object IDataRecord.this[string name] => _dataReader[name];

        object IDataRecord.this[int i] => _dataReader[i];

#endregion
    }
}