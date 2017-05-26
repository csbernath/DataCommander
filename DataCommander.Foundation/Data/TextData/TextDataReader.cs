using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataReader : DbDataReader
    {
        #region Private Fields

        private TextDataCommand _command;
        private readonly CommandBehavior _behavior;
        private readonly TextDataColumnCollection _columns;
        private DataTable _schemaTable;
        private readonly TextReader _textReader;
        private readonly TextDataStreamReader _textDataStreamReader;
        private object[] _values;
        private int _rowCount;

        #endregion

        internal TextDataReader( TextDataCommand command, CommandBehavior behavior )
        {
#if CONTRACTS_FULL
            Contract.Requires(command != null);
#endif

            this._command = command;
            this._behavior = behavior;
            var parameters = command.Parameters;
#if CONTRACTS_FULL
            Contract.Assert(parameters != null);
#endif

            this._columns = parameters.GetParameterValue<TextDataColumnCollection>( "columns" );
            var converters = parameters.GetParameterValue<IList<ITextDataConverter>>( "converters" );
            var getTextReader = parameters.GetParameterValue<IConverter<TextDataCommand, TextReader>>( "getTextReader" );
            this._textReader = getTextReader.Convert( command );
            this._textDataStreamReader = new TextDataStreamReader( this._textReader, this._columns, converters );
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
        public override int Depth => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override int FieldCount => this._columns.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool GetBoolean( int ordinal )
        {
            return (bool) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override byte GetByte( int ordinal )
        {
            return (byte) this._values[ ordinal ];
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
        public override char GetChar( int ordinal )
        {
            return (char) this._values[ ordinal ];
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
        public override long GetChars( int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length )
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
            return (DateTime) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override decimal GetDecimal( int ordinal )
        {
            return (decimal) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override double GetDouble( int ordinal )
        {
            return (double) this._values[ ordinal ];
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
        public override float GetFloat( int ordinal )
        {
            return (float) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Guid GetGuid( int ordinal )
        {
            return (Guid) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override short GetInt16( int ordinal )
        {
            return (short) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override int GetInt32( int ordinal )
        {
            return (int) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override long GetInt64( int ordinal )
        {
            return (long) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetName( int ordinal )
        {
            var column = this._columns[ ordinal ];
            return column.ColumnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override int GetOrdinal( string name )
        {
            return this._columns.IndexOf( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            if (this._schemaTable == null)
            {
                this._schemaTable = new DataTable();
                this._schemaTable.Locale = CultureInfo.InvariantCulture;
                this._schemaTable.Columns.Add( "ColumnName", typeof( string ) );
                this._schemaTable.Columns.Add( "DataType", typeof( Type ) );
                this._schemaTable.Columns.Add( "IsKey", typeof( bool ) );

                foreach (var column in this._columns)
                {
                    object[] values =
                    {
                        column.ColumnName,
                        column.DataType,
                        false
                    };

                    this._schemaTable.Rows.Add( values );
                }
            }

            return this._schemaTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetString( int ordinal )
        {
            return (string) this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object GetValue( int ordinal )
        {
            return this._values[ ordinal ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int GetValues( object[] values )
        {
            this._values.CopyTo( values, 0 );
            return values.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool HasRows => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsClosed => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override bool IsDBNull( int ordinal )
        {
            return this._values[ ordinal ] == DBNull.Value;
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

            if (this._behavior == CommandBehavior.SingleRow && this._rowCount == 1)
            {
                read = false;
            }
            else
            {
                this._values = this._textDataStreamReader.ReadRow();
                read = this._values != null;

                if (read)
                {
                    this._rowCount++;
                }
            }

            return read;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int RecordsAffected => this._rowCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[ string name ]
        {
            get
            {
                var index = this._columns.IndexOf( name, true );
                return this._values[ index ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override object this[ int ordinal ] => this._values[ ordinal ];
    }
}