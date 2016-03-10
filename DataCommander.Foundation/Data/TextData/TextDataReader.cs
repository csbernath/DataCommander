namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections;
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
        private readonly CommandBehavior behavior;
        private readonly TextDataColumnCollection columns;
        private DataTable schemaTable;
        private readonly TextReader textReader;
        private readonly TextDataStreamReader textDataStreamReader;
        private object[] values;
        private int rowCount;

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
        public override int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int FieldCount => this.columns.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool GetBoolean( int ordinal )
        {
            return (bool) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override byte GetByte( int ordinal )
        {
            return (byte) this.values[ ordinal ];
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
            return (DateTime) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override decimal GetDecimal( int ordinal )
        {
            return (decimal) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Double GetDouble( int ordinal )
        {
            return (Double) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Single GetFloat( int ordinal )
        {
            return (Single) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid( int ordinal )
        {
            return (Guid) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Int16 GetInt16( int ordinal )
        {
            return (Int16) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override int GetInt32( int ordinal )
        {
            return (int) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override long GetInt64( int ordinal )
        {
            return (long) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetName( int ordinal )
        {
            TextDataColumn column = this.columns[ ordinal ];
            return column.ColumnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override int GetOrdinal( string name )
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
                this.schemaTable.Columns.Add( "ColumnName", typeof( string ) );
                this.schemaTable.Columns.Add( "DataType", typeof( Type ) );
                this.schemaTable.Columns.Add( "IsKey", typeof( bool ) );

                foreach (TextDataColumn column in this.columns)
                {
                    object[] values =
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
        public override string GetString( int ordinal )
        {
            return (string) this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object GetValue( int ordinal )
        {
            return this.values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int GetValues( object[] values )
        {
            this.values.CopyTo( values, 0 );
            return values.Length;
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
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool IsDBNull( int ordinal )
        {
            return this.values[ ordinal ] == DBNull.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            bool read;

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
        public override int RecordsAffected => this.rowCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[ string name ]
        {
            get
            {
                int index = this.columns.IndexOf( name, true );
                return this.values[ index ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object this[ int ordinal ] => this.values[ ordinal ];
    }
}