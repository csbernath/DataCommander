namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal sealed class LoggedDataReader : IDataReader
    {
        private readonly IDataReader dataReader;
        private EventHandler<AfterReadEventArgs> afterRead;
        private Int32 rowCount;

        public LoggedDataReader(
            IDataReader dataReader,
            EventHandler<AfterReadEventArgs> afterRead )
        {
            Contract.Requires( dataReader != null );
            Contract.Requires( afterRead != null );

            this.dataReader = dataReader;
            this.afterRead = afterRead;
        }

        #region IDataReader Members

        void IDataReader.Close()
        {
            this.dataReader.Close();
        }

        Int32 IDataReader.Depth
        {
            get
            {
                return this.dataReader.Depth;
            }
        }

        DataTable IDataReader.GetSchemaTable()
        {
            return this.dataReader.GetSchemaTable();
        }

        bool IDataReader.IsClosed
        {
            get
            {
                return this.dataReader.IsClosed;
            }
        }

        bool IDataReader.NextResult()
        {
            return this.dataReader.NextResult();
        }

        bool IDataReader.Read()
        {
            Boolean read = this.dataReader.Read();
            if (read)
            {
                this.rowCount++;
            }
            else if (this.afterRead != null)
            {
                var eventArgs = new AfterReadEventArgs( this.rowCount );
                this.afterRead( this, eventArgs );
            }

            return read;
        }

        Int32 IDataReader.RecordsAffected
        {
            get
            {
                return this.dataReader.RecordsAffected;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.dataReader.Dispose();
        }

        #endregion

        #region IDataRecord Members

        Int32 IDataRecord.FieldCount
        {
            get
            {
                return this.dataReader.FieldCount;
            }
        }

        bool IDataRecord.GetBoolean( Int32 i )
        {
            return this.dataReader.GetBoolean( i );
        }

        byte IDataRecord.GetByte( Int32 i )
        {
            return this.dataReader.GetByte( i );
        }

        Int64 IDataRecord.GetBytes( Int32 i, Int64 fieldOffset, byte[] buffer, Int32 bufferoffset, Int32 length )
        {
            return this.dataReader.GetBytes( i, fieldOffset, buffer, bufferoffset, length );
        }

        char IDataRecord.GetChar( Int32 i )
        {
            return this.dataReader.GetChar( i );
        }

        Int64 IDataRecord.GetChars( Int32 i, Int64 fieldoffset, char[] buffer, Int32 bufferoffset, Int32 length )
        {
            return this.dataReader.GetChars( i, fieldoffset, buffer, bufferoffset, length );
        }

        IDataReader IDataRecord.GetData( Int32 i )
        {
            return this.dataReader.GetData( i );
        }

        string IDataRecord.GetDataTypeName( Int32 i )
        {
            return this.dataReader.GetDataTypeName( i );
        }

        DateTime IDataRecord.GetDateTime( Int32 i )
        {
            return this.dataReader.GetDateTime( i );
        }

        decimal IDataRecord.GetDecimal( Int32 i )
        {
            return this.dataReader.GetDecimal( i );
        }

        double IDataRecord.GetDouble( Int32 i )
        {
            return this.dataReader.GetDouble( i );
        }

        Type IDataRecord.GetFieldType( Int32 i )
        {
            return this.dataReader.GetFieldType( i );
        }

        float IDataRecord.GetFloat( Int32 i )
        {
            return this.dataReader.GetFloat( i );
        }

        Guid IDataRecord.GetGuid( Int32 i )
        {
            return this.dataReader.GetGuid( i );
        }

        short IDataRecord.GetInt16( Int32 i )
        {
            return this.dataReader.GetInt16( i );
        }

        Int32 IDataRecord.GetInt32( Int32 i )
        {
            return this.dataReader.GetInt32( i );
        }

        Int64 IDataRecord.GetInt64( Int32 i )
        {
            return this.dataReader.GetInt64( i );
        }

        string IDataRecord.GetName( Int32 i )
        {
            return this.dataReader.GetName( i );
        }

        Int32 IDataRecord.GetOrdinal( string name )
        {
            return this.dataReader.GetOrdinal( name );
        }

        string IDataRecord.GetString( Int32 i )
        {
            return this.dataReader.GetString( i );
        }

        object IDataRecord.GetValue( Int32 i )
        {
            return this.dataReader.GetValue( i );
        }

        Int32 IDataRecord.GetValues( object[] values )
        {
            return this.dataReader.GetValues( values );
        }

        bool IDataRecord.IsDBNull( Int32 i )
        {
            return this.dataReader.IsDBNull( i );
        }

        object IDataRecord.this[ string name ]
        {
            get
            {
                return this.dataReader[ name ];
            }
        }

        object IDataRecord.this[ Int32 i ]
        {
            get
            {
                return this.dataReader[ i ];
            }
        }

        #endregion
    }
}