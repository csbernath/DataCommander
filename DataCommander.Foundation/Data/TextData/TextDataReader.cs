namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;    

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataReader : DbDataReader
    {
        #region Private Fields

        private TextDataCommand command;
        private CommandBehavior behavior;
        private TextDataColumnCollection columns;
        private DataTable schemaTable;
        private TextReader textReader;
        private TextDataStreamReader textDataStreamReader;
        private Object[] values;
        private Int32 rowCount;

        #endregion

        internal TextDataReader( TextDataCommand command, CommandBehavior behavior )
        {
            Contract.Requires(command != null);

            this.command = command;
            this.behavior = behavior;
            TextDataParameterCollection parameters = command.Parameters;
            Contract.Assert(parameters != null);

            this.columns = parameters.GetParameterValue<TextDataColumnCollection>( "columns" );
            IList<ITextDataConverter> converters = parameters.GetParameterValue<IList<ITextDataConverter>>( "converters" );
            IConverter<TextDataCommand, TextReader> getTextReader = parameters.GetParameterValue<IConverter<TextDataCommand, TextReader>>( "getTextReader" );
            this.textReader = getTextReader.Convert( command );
            this.textDataStreamReader = new TextDataStreamReader( this.textReader, this.columns, converters );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 FieldCount
        {
            get
            {
                return this.columns.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Boolean GetBoolean( Int32 ordinal )
        {
            return (Boolean) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Byte GetByte( Int32 ordinal )
        {
            return (Byte) this.values[ ordinal ];
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
        public override Int64 GetBytes( Int32 ordinal, Int64 dataOffset, Byte[] buffer, Int32 bufferOffset, Int32 length )
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
            return (Char) this.values[ ordinal ];
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
            return (DateTime) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Decimal GetDecimal( Int32 ordinal )
        {
            return (Decimal) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Double GetDouble( Int32 ordinal )
        {
            return (Double) this.values[ ordinal ];
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Single GetFloat( Int32 ordinal )
        {
            return (Single) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid( Int32 ordinal )
        {
            return (Guid) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int16 GetInt16( Int32 ordinal )
        {
            return (Int16) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int32 GetInt32( Int32 ordinal )
        {
            return (Int32) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int64 GetInt64( Int32 ordinal )
        {
            return (Int64) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override String GetName( Int32 ordinal )
        {
            TextDataColumn column = this.columns[ ordinal ];
            return column.ColumnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Int32 GetOrdinal( String name )
        {
            return this.columns.IndexOf( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            if (this.schemaTable == null)
            {
                this.schemaTable = new DataTable();
                this.schemaTable.Locale = CultureInfo.InvariantCulture;
                this.schemaTable.Columns.Add( "ColumnName", typeof( String ) );
                this.schemaTable.Columns.Add( "DataType", typeof( Type ) );
                this.schemaTable.Columns.Add( "IsKey", typeof( Boolean ) );

                foreach (TextDataColumn column in this.columns)
                {
                    Object[] values =
                    {
                        column.ColumnName,
                        column.DataType,
                        false
                    };

                    this.schemaTable.Rows.Add( values );
                }
            }

            return this.schemaTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override String GetString( Int32 ordinal )
        {
            return (String) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Object GetValue( Int32 ordinal )
        {
            return this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override Int32 GetValues( Object[] values )
        {
            this.values.CopyTo( values, 0 );
            return values.Length;
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
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Boolean IsDBNull( Int32 ordinal )
        {
            return this.values[ ordinal ] == DBNull.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Boolean NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Boolean Read()
        {
            Boolean read;

            if (this.behavior == CommandBehavior.SingleRow && this.rowCount == 1)
            {
                read = false;
            }
            else
            {
                this.values = this.textDataStreamReader.ReadRow();
                read = this.values != null;

                if (read)
                {
                    this.rowCount++;
                }
            }

            return read;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 RecordsAffected
        {
            get
            {
                return this.rowCount;
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
                Int32 index = this.columns.IndexOf( name, true );
                return this.values[ index ];
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
                return this.values[ ordinal ];
            }
        }
    }
}