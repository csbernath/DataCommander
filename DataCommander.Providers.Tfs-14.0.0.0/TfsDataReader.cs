namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Data.Common;

    internal abstract class TfsDataReader : IDataReader
    {
        private object[] values;

        #region IDataReader Members

        public void Close()
        {
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public abstract DataTable GetSchemaTable();

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public bool NextResult()
        {
            return false;
        }

        public abstract bool Read();

        public abstract int RecordsAffected
        {
            get;
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region IDataRecord Members

        public abstract int FieldCount
        {
            get;
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return null;
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            this.values.CopyTo(values, 0);
            return this.values.Length;
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        internal object[] Values
        {
            get
            {
                return this.values;
            }

            set
            {
                this.values = value;
            }
        }

        internal static DataTable CreateSchemaTable()
        {
            DataTable table = new DataTable();
            DataColumnCollection columns = table.Columns;
            columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            columns.Add("ColumnSize", typeof(int));
            columns.Add("DataType", typeof(Type));
            columns.Add("ProviderType", typeof(int));
            columns.Add("AllowDBNull", typeof(bool));
            return table;
        }

        internal static void AddSchemaRowBoolean(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(new object[]
            {
                name,
                sizeof(bool),
                typeof(bool),
                DbType.Boolean,
                allowDBNull,
            });
        }

        internal static void AddSchemaRowDateTime(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(new object[]
            {
                name,
                8,
                typeof(DateTime),
                DbType.DateTime,
                allowDBNull
            });
        }

        internal static void AddSchemaRowString(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(new object[]
            {
                name,
                8096,
                typeof(string),
                DbType.String,
                allowDBNull
            });
        }

        internal static void AddSchemaRowInt32(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(new object[]
            {
                name,
                sizeof(int),
                typeof(int),
                DbType.Int32,
                allowDBNull
            });
        }

        internal static void AddSchemaRowInt64(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(new object[]
            {
                name,
                sizeof(long),
                typeof(long),
                DbType.Int64,
                allowDBNull
            });
        }
    }
}