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
    public delegate Int32 GetValues( Object[] values );

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
        public override Int32 Depth
        {
            get
            {
                return this.dataReader.Depth;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 FieldCount
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
        public override Boolean GetBoolean( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override byte GetByte( Int32 ordinal )
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
        public override Int64 GetBytes( Int32 ordinal, Int64 dataOffset, byte[] buffer, Int32 bufferOffset, Int32 length )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Char GetChar( Int32 ordinal )
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
        public override Int64 GetChars( Int32 ordinal, Int64 dataOffset, Char[] buffer, Int32 bufferOffset, Int32 length )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override String GetDataTypeName( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override DateTime GetDateTime( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Decimal GetDecimal( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override double GetDouble( Int32 ordinal )
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
        public override Type GetFieldType( Int32 ordinal )
        {
            return this.dataReader.GetFieldType( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override float GetFloat( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int16 GetInt16( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int32 GetInt32( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int64 GetInt64( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override String GetName( Int32 ordinal )
        {
            return this.dataReader.GetName( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Int32 GetOrdinal( String name )
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
        public override String GetString( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Object GetValue( Int32 ordinal )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override Int32 GetValues( Object[] values )
        {
            return this.getValues( values );
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean HasRows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsClosed
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
        public override Boolean IsDBNull( Int32 ordinal )
        {
            return this.dataReader.IsDBNull( ordinal );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Boolean NextResult()
        {
            return this.dataReader.NextResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Boolean Read()
        {
            return this.dataReader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 RecordsAffected
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
        public override Object this[ String name ]
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
        public override Object this[ Int32 ordinal ]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}