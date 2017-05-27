using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Deployment.WindowsInstaller;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiDataReader : IDataReader
    {
        private readonly MsiCommand _command;
        private CommandBehavior _behavior;
        private IEnumerator<Record> _enumerator;
        private int _recordsAffected;

        public MsiDataReader(MsiCommand command, CommandBehavior behavior)
        {
#if CONTRACTS_FULL
            Contract.Requires( command != null );
#endif
            _command = command;
            _behavior = behavior;
            View = _command.Connection.Database.OpenView(_command.CommandText);
        }

        public View View { get; }

        #region IDataReader Members

        void IDataReader.Close()
        {
            if (View != null)
            {
                View.Close();
            }
        }

        int IDataReader.Depth => throw new NotImplementedException();

        public DataTable GetSchemaTable()
        {
            var table = new DataTable();
            var columns = table.Columns;
            columns.Add("ColumnName", typeof(string));
            columns.Add("ColumnSize", typeof(int));
            columns.Add("DataType", typeof(Type));
            columns.Add("ProviderType", typeof(int));
            columns.Add("Definition", typeof(string));

            foreach (var column in View.Columns)
            {
                table.Rows.Add(new object[]
                {
                    column.Name,
                    column.Size,
                    column.Type,
                    (int) column.DBType,
                    column.ColumnDefinitionString
                });
            }

            return table;
        }

        bool IDataReader.IsClosed => throw new NotImplementedException();

        bool IDataReader.NextResult()
        {
            return false;
        }

        bool IDataReader.Read()
        {
            if (_enumerator == null)
            {
                View.Execute();
                _enumerator = View.GetEnumerator();
            }

            var read = _enumerator.MoveNext();

            if (read)
            {
                _recordsAffected++;
            }

            return read;
        }

        int IDataReader.RecordsAffected => _recordsAffected;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
            }

            if (View != null)
            {
                View.Dispose();
            }
        }

        #endregion

        #region IDataRecord Members

        int IDataRecord.FieldCount => View.Columns.Count;

        bool IDataRecord.GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        byte IDataRecord.GetByte(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        char IDataRecord.GetChar(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            var column = View.Columns[i];
            var dbType = (DbType) column.DBType;
            var dataTypeName = dbType.ToString();
            return dataTypeName;
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        double IDataRecord.GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        Type IDataRecord.GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        float IDataRecord.GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        Guid IDataRecord.GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        short IDataRecord.GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetName(int i)
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            var record = _enumerator.Current;
            return record[i + 1];
        }

        public int GetValues(object[] values)
        {
            int count;

            if (values != null)
            {
                var record = _enumerator.Current;
                count = Math.Min(values.Length, record.FieldCount);

                for (var i = 0; i < count; i++)
                {
                    values[i] = record[i + 1];
                }
            }
            else
            {
                count = 0;
            }

            return count;
        }

        bool IDataRecord.IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        object IDataRecord.this[string name] => throw new NotImplementedException();

        object IDataRecord.this[int i] => GetValue(i);

        #endregion
    }
}