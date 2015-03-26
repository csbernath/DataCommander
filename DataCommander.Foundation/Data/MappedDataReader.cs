namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public delegate int GetValues( object[] values );

    /// <summary>
    /// 
    /// </summary>
    public class MappedDataReader : DbDataReader
    {
        private IDataReader dataReader;
        private GetValues getValues;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="getValues"></param>
        public MappedDataReader( IDataReader dataReader, GetValues getValues )
        {
            Contract.Requires( dataReader != null );
            Contract.Requires( getValues != null );

            this.dataReader = dataReader;
            this.getValues = getValues;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            this.dataReader.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Depth
        {
            get
            {
                return this.dataReader.Depth;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int FieldCount
        {
            get
            {
                return this.dataReader.FieldCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool GetBoolean( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override byte GetByte( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="dataOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetBytes( int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Char GetChar( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="dataOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetChars( int ordinal, long dataOffset, Char[] buffer, int bufferOffset, int length )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetDataTypeName( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override DateTime GetDateTime( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Decimal GetDecimal( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override double GetDouble( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Type GetFieldType( int ordinal )
        {
            return this.dataReader.GetFieldType( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override float GetFloat( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int16 GetInt16( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override int GetInt32( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override long GetInt64( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetName( int ordinal )
        {
            return this.dataReader.GetName( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override int GetOrdinal( string name )
        {
            return this.dataReader.GetOrdinal( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            return this.dataReader.GetSchemaTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetString( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object GetValue( int ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int GetValues( object[] values )
        {
            return this.getValues( values );
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool HasRows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsClosed
        {
            get
            {
                return this.dataReader.IsClosed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool IsDBNull( int ordinal )
        {
            return this.dataReader.IsDBNull( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool NextResult()
        {
            return this.dataReader.NextResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            return this.dataReader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int RecordsAffected
        {
            get
            {
                return this.dataReader.RecordsAffected;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[ string name ]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object this[ int ordinal ]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}