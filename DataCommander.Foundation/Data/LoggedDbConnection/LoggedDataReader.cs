namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal sealed class LoggedDataReader : IDataReader
    {
        private readonly IDataReader dataReader;
        private readonly EventHandler<AfterReadEventArgs> afterRead;
        private int rowCount;

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

        int IDataReader.Depth => this.dataReader.Depth;

        DataTable IDataReader.GetSchemaTable()
        {
            return this.dataReader.GetSchemaTable();
        }

        bool IDataReader.IsClosed => this.dataReader.IsClosed;

        bool IDataReader.NextResult()
        {
            return this.dataReader.NextResult();
        }

        bool IDataReader.Read()
        {
            var read = this.dataReader.Read();
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

        int IDataReader.RecordsAffected => this.dataReader.RecordsAffected;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.dataReader.Dispose();
        }

        #endregion

        #region IDataRecord Members

        int IDataRecord.FieldCount => this.dataReader.FieldCount;

        bool IDataRecord.GetBoolean( int i )
        {
            return this.dataReader.GetBoolean( i );
        }

        byte IDataRecord.GetByte( int i )
        {
            return this.dataReader.GetByte( i );
        }

        long IDataRecord.GetBytes( int i, long fieldOffset, byte[] buffer, int bufferoffset, int length )
        {
            return this.dataReader.GetBytes( i, fieldOffset, buffer, bufferoffset, length );
        }

        char IDataRecord.GetChar( int i )
        {
            return this.dataReader.GetChar( i );
        }

        long IDataRecord.GetChars( int i, long fieldoffset, char[] buffer, int bufferoffset, int length )
        {
            return this.dataReader.GetChars( i, fieldoffset, buffer, bufferoffset, length );
        }

        IDataReader IDataRecord.GetData( int i )
        {
            return this.dataReader.GetData( i );
        }

        string IDataRecord.GetDataTypeName( int i )
        {
            return this.dataReader.GetDataTypeName( i );
        }

        DateTime IDataRecord.GetDateTime( int i )
        {
            return this.dataReader.GetDateTime( i );
        }

        decimal IDataRecord.GetDecimal( int i )
        {
            return this.dataReader.GetDecimal( i );
        }

        double IDataRecord.GetDouble( int i )
        {
            return this.dataReader.GetDouble( i );
        }

        Type IDataRecord.GetFieldType( int i )
        {
            return this.dataReader.GetFieldType( i );
        }

        float IDataRecord.GetFloat( int i )
        {
            return this.dataReader.GetFloat( i );
        }

        Guid IDataRecord.GetGuid( int i )
        {
            return this.dataReader.GetGuid( i );
        }

        short IDataRecord.GetInt16( int i )
        {
            return this.dataReader.GetInt16( i );
        }

        int IDataRecord.GetInt32( int i )
        {
            return this.dataReader.GetInt32( i );
        }

        long IDataRecord.GetInt64( int i )
        {
            return this.dataReader.GetInt64( i );
        }

        string IDataRecord.GetName( int i )
        {
            return this.dataReader.GetName( i );
        }

        int IDataRecord.GetOrdinal( string name )
        {
            return this.dataReader.GetOrdinal( name );
        }

        string IDataRecord.GetString( int i )
        {
            return this.dataReader.GetString( i );
        }

        object IDataRecord.GetValue( int i )
        {
            return this.dataReader.GetValue( i );
        }

        int IDataRecord.GetValues( object[] values )
        {
            return this.dataReader.GetValues( values );
        }

        bool IDataRecord.IsDBNull( int i )
        {
            return this.dataReader.IsDBNull( i );
        }

        object IDataRecord.this[ string name ] => this.dataReader[ name ];

        object IDataRecord.this[ int i ] => this.dataReader[ i ];

        #endregion
    }
}