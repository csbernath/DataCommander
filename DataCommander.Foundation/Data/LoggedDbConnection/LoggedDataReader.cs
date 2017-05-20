namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    using System;
    using System.Data;

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

            this._dataReader = dataReader;
            this._afterRead = afterRead;
        }

#region IDataReader Members

        void IDataReader.Close()
        {
            this._dataReader.Close();
        }

        int IDataReader.Depth => this._dataReader.Depth;

        DataTable IDataReader.GetSchemaTable()
        {
            return this._dataReader.GetSchemaTable();
        }

        bool IDataReader.IsClosed => this._dataReader.IsClosed;

        bool IDataReader.NextResult()
        {
            return this._dataReader.NextResult();
        }

        bool IDataReader.Read()
        {
            var read = this._dataReader.Read();
            if (read)
            {
                this._rowCount++;
            }
            else if (this._afterRead != null)
            {
                var eventArgs = new AfterReadEventArgs(this._rowCount);
                this._afterRead(this, eventArgs);
            }

            return read;
        }

        int IDataReader.RecordsAffected => this._dataReader.RecordsAffected;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this._dataReader.Dispose();
        }

#endregion

#region IDataRecord Members

        int IDataRecord.FieldCount => this._dataReader.FieldCount;

        bool IDataRecord.GetBoolean(int i)
        {
            return this._dataReader.GetBoolean(i);
        }

        byte IDataRecord.GetByte(int i)
        {
            return this._dataReader.GetByte(i);
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        char IDataRecord.GetChar(int i)
        {
            return this._dataReader.GetChar(i);
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            return this._dataReader.GetData(i);
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            return this._dataReader.GetDataTypeName(i);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            return this._dataReader.GetDateTime(i);
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            return this._dataReader.GetDecimal(i);
        }

        double IDataRecord.GetDouble(int i)
        {
            return this._dataReader.GetDouble(i);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            return this._dataReader.GetFieldType(i);
        }

        float IDataRecord.GetFloat(int i)
        {
            return this._dataReader.GetFloat(i);
        }

        Guid IDataRecord.GetGuid(int i)
        {
            return this._dataReader.GetGuid(i);
        }

        short IDataRecord.GetInt16(int i)
        {
            return this._dataReader.GetInt16(i);
        }

        int IDataRecord.GetInt32(int i)
        {
            return this._dataReader.GetInt32(i);
        }

        long IDataRecord.GetInt64(int i)
        {
            return this._dataReader.GetInt64(i);
        }

        string IDataRecord.GetName(int i)
        {
            return this._dataReader.GetName(i);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            return this._dataReader.GetOrdinal(name);
        }

        string IDataRecord.GetString(int i)
        {
            return this._dataReader.GetString(i);
        }

        object IDataRecord.GetValue(int i)
        {
            return this._dataReader.GetValue(i);
        }

        int IDataRecord.GetValues(object[] values)
        {
            return this._dataReader.GetValues(values);
        }

        bool IDataRecord.IsDBNull(int i)
        {
            return this._dataReader.IsDBNull(i);
        }

        object IDataRecord.this[string name] => this._dataReader[name];

        object IDataRecord.this[int i] => this._dataReader[i];

#endregion
    }
}