namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Data.Common;

    internal abstract class TfsDataReader : IDataReader
    {
        #region IDataReader Members

        public void Close()
        {
        }

        public int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public abstract DataTable GetSchemaTable();

        public bool IsClosed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool NextResult()
        {
            return false;
        }

        public abstract bool Read();

        public abstract int RecordsAffected { get; }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region IDataRecord Members

        public abstract int FieldCount { get; }

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
            this.Values.CopyTo(values, 0);
            return this.Values.Length;
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public object this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object this[int i]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        internal object[] Values { get; set; }

        internal static DataTable CreateSchemaTable()
        {
            var table = new DataTable();
            var columns = table.Columns;
            columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
            columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
            columns.Add(SchemaTableColumn.NumericPrecision, typeof(short));
            columns.Add(SchemaTableColumn.NumericScale, typeof(short));
            columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
            columns.Add(SchemaTableColumn.IsKey, typeof(bool));
            //BaseServerName
            //BaseCatalogName
            columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
            columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
            columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
            columns.Add(SchemaTableColumn.DataType, typeof(Type));
            columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
            columns.Add(SchemaTableColumn.ProviderType, typeof(int));
            columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
            columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
            //IsIdentity
            //IsAutoIncrement
            //IsRowVersion
            //IsHidden
            columns.Add(SchemaTableColumn.IsLong, typeof(bool));
            //IsReadOnly
            //ProviderSpecificDataType
            //DataTypeName XmlSchemaCollectionDatabase XmlSchemaCollectionOwningSchema XmlSchemaCollectionName UdtAssemblyQualifiedName NonVersionedProviderType IsColumnSet

            return table;
        }

        private static object[] CreateDataRowValues(string columnName, int columnSize, Type dataType, DbType providerType, bool allowDBNull)
        {
            return new object[]
            {
                columnName,
                0,
                columnSize,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                dataType,
                allowDBNull,
                providerType,
                null,
                null,
                null
            };
        }

        internal static void AddSchemaRowBoolean(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(CreateDataRowValues(name, sizeof(bool), typeof(bool), DbType.Boolean, allowDBNull));
        }

        internal static void AddSchemaRowDateTime(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(CreateDataRowValues(name, 8, typeof(DateTime), DbType.DateTime, allowDBNull));
        }

        internal static void AddSchemaRowString(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(CreateDataRowValues(name, 8096, typeof(string), DbType.String, allowDBNull));
        }

        internal static void AddSchemaRowInt32(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(CreateDataRowValues(name, sizeof(int), typeof(int), DbType.Int32, allowDBNull));
        }

        internal static void AddSchemaRowInt64(DataTable schemaTable, string name, bool allowDBNull)
        {
            schemaTable.Rows.Add(CreateDataRowValues(name, sizeof(long), typeof(long), DbType.Int64, allowDBNull));
        }
    }
}